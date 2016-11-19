namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Runtime.InteropServices;

    using static System.FormattableString;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Tasks;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about the commits within a repository.
    /// </summary>
    public class GetCommitInfo : Task
    {
        #region Constants
        private const string Split = "------------------------ >8 ------------------------";

        private static readonly bool IsWinblows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets the commit from which to retrieve commit history.
        /// </summary>
        /// <remarks>
        /// The default value is <see langword="null"/>, which will cause all commits in the entire repository to be
        /// included.
        /// </remarks>
        [Output]
        public string From { get; set; }

        /// <summary>
        /// Gets the commit to which to retrieve commit history.
        /// </summary>
        /// <remarks>
        /// The default value is HEAD, which is the most recent commit.
        /// </remarks>
        [Output]
        public string To { get; set; } = "HEAD";

        /// <summary>
        /// Gets or sets the actions used to track references that resolve an issue (work item).
        /// </summary>
        /// <remarks>
        /// The item specification of each task item within the collection will be used to determine whether an issue
        /// (work item) is resolved by the commit. If an issue (work item) reference is not found on a line that starts
        /// with one of these terms, it will not be included as a resolution.
        /// </remarks>
        public ITaskItem[] Actions { get; set; } = new[]
            {
                new TaskItem("close"),
                new TaskItem("closes"),
                new TaskItem("closed"),
                new TaskItem("fix"),
                new TaskItem("fixed"),
                new TaskItem("resolve"),
                new TaskItem("resolves"),
                new TaskItem("resolved")
            };

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse the header.
        /// </summary>
        /// <remarks>
        /// The position and number of match groups within the pattern are directly related to the
        /// <see cref="HeaderCorrespondence"/> whereby the index of the match group must match the index of the
        /// correspondence task item.
        /// </remarks>
        public string HeaderPattern { get; set; } = @"^(\w*)(?:\(([\w\$\.\-\* ]*)\))?\: (.*)$";

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a field within a commit message.
        /// </summary>
        public string FieldPattern { get; set; } = @"^-(.*?)-$";

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a reversion within a commit message.
        /// </summary>
        public string RevertPattern { get; set; } = @"^Revert\s""([\s\S]*)""\s*This reverts commit (\w*)\.";

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a merge within a commit message.
        /// </summary>
        public string MergePattern { get; set; }

        /// <summary>
        /// Gets or sets the correspondence for a header.
        /// </summary>
        public ITaskItem[] HeaderCorrespondence { get; set; } = new[]
            {
                new TaskItem("type", new Dictionary<string, string> { { "Name", "Type" } }),
                new TaskItem("scope", new Dictionary<string, string> { { "Name", "Scope" } }),
                new TaskItem("subject", new Dictionary<string, string> { { "Name", "Subject" } }),
            };

        /// <summary>
        /// Gets or sets the correspondence for a merge.
        /// </summary>
        public ITaskItem[] MergeCorrespondence { get; set; }

        /// <summary>
        /// Gets or sets the prefixes used to reference an issue (work item) within a commit message.
        /// </summary>
        public ITaskItem[] IssuePrefixes { get; set; } = new[]
            {
                new TaskItem("#")
            };

        /// <summary>
        /// Gets or sets the keywords used to reference a breaking change within a commit message.
        /// </summary>
        public ITaskItem[] NoteKeywords { get; set; } = new[]
            {
                new TaskItem("BREAKING CHANGE")
            };

        /// <summary>
        /// Gets the commits that belong to the current release.
        /// </summary>
        /// <remarks>
        /// This list contains the commit SHA's of all commits that have occurred after the commit associated with
        /// the <see cref="LatestTag"/> property. The SHA value will be the <see cref="ITaskItem.ItemSpec"/>. In
        /// addition, the task items will contain metadata that describes any commit that follows the
        /// conventional-changelog style guide. This metadata includes the following:
        /// <list type="bullet">
        /// <item>
        ///     <term>Type</term>
        ///     <description>
        ///     The type (of change) for the commit, which may be Features, Performance Improvements, Bug Fixes,
        ///     Documentation, Styles, Code Refactoring, Tests or Chores.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Scope</term>
        ///     <description>
        ///     The component in which the change occurred.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Subject</term>
        ///     <description>
        ///     The subject, or description, of the change that occurred.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Body</term>
        ///     <description>
        ///     The body that describes additional details about the change that occurred.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Closes</term>
        ///     <description>
        ///     A semi-colon (;) delimited list of issues (work items) that are closed (resolved) by the commit.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>References</term>
        ///     <description>
        ///     A semi-colon (;) delimited list of references (work items) that are related to the commit.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Breaking</term>
        ///     <description>
        ///     A summary description of breaking changes associated with the commit.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Message</term>
        ///     <description>
        ///     The entire commit message including new-lines. This is useful for commits that do not follow the
        ///     conventional changelog style.
        ///     </description>
        /// </list>
        /// </remarks>
        [Output]
        public ITaskItem[] Commits { get; set; }

        /// <summary>
        /// Gets the latest release tag contained within the repository.
        /// </summary>
        [Output]
        public string LatestTag { get; private set; }

        /// <summary>
        /// Gets the commit hash of the latest tag commit.
        /// </summary>
        [Output]
        public string LatestTagCommit { get; private set; }

        /// <summary>
        /// Gets the version of the client used to access the repository.
        /// </summary>
        [Output]
        public string ClientVersion { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetRepositoryInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // set the root
            var root = this.RepositoryRoot;

            // determine if the root could be found
            if (string.IsNullOrEmpty(root))
            {
                // write a warning
                this.Log.LogWarning(Invariant($"The {nameof(RepositoryRoot)} property was not specified."));

                // move on immediately
                return true;
            }

            // determine if the directory exists
            if (!Directory.Exists(root))
            {
                // write a warning
                this.Log.LogWarning(Invariant($"The repository root ({root}) does not exist."));

                // move on immediately
                return true;
            }

            var gitconfig = Path.Combine(root, ".git");

            // determine if the git config path exists
            if (!Directory.Exists(gitconfig))
            {
                // write a warning
                this.Log.LogWarning(Invariant($"The repository root ({root}) is not a git repository."));

                // move on immediately
                return true;
            }

            // create an exec task for retrieving the version
            var exec = this.CreateExecTask("--version", root);

            // execute the command and ensure that the output exists
            if (!exec.Execute() || exec.ConsoleOutput.Length == 0)
            {
                // write a warning
                this.Log.LogWarning(Invariant($"Git cannot be found on the current path."));

                // move on immediately
                return true;
            }

            // set the client version
            this.ClientVersion = exec.ConsoleOutput[0].ItemSpec;

            // determine if the latest tag commit is empty
            if (string.IsNullOrEmpty(this.LatestTagCommit))
            {
                // create the command used to get the latest tag commit
                exec = this.CreateExecTask("rev-list --tags --max-count=1", root);

                // execute the command and ensure that the output contains a result
                if (exec.Execute() && exec.ConsoleOutput.Length == 1)
                {
                    // set the tag commit
                    this.LatestTagCommit = exec.ConsoleOutput[0].ItemSpec;

                    // create the command used to describe the commit
                    exec = this.CreateExecTask(Invariant($"describe --tags {this.LatestTagCommit}"), root);

                    // execute the command and ensure that the output contains a result
                    if (exec.Execute() && exec.ConsoleOutput.Length == 1)
                    {
                        // set the tag
                        this.LatestTag = exec.ConsoleOutput[0].ItemSpec;
                    }
                }
            }

            // get the commits
            this.Commits = this.GetCommits().ToArray();

            // we were successful
            return true;
        }

        private IEnumerable<ITaskItem> GetCommits()
        {
            // create the range
            var range = string.IsNullOrEmpty(this.From) ? this.To : $"{this.From}..{this.To}";

            // create the command used to get the history of commits
            var exec = this.CreateExecTask($@"log {range} --format=""%H%n%h%n%s%n%b%n{Split}%n""", this.RepositoryRoot);

            // execute the command and ensure results
            if (!exec.Execute() || exec.ConsoleOutput.Length == 0)
            {
                // move on immediately
                return new ITaskItem[0];
            }

            // capture the lines from the console output
            var lines = exec.ConsoleOutput.Select(item => item.ItemSpec);

            // parse the lines
            return this.ParseCommits(lines);
        }

        private IEnumerable<ITaskItem> ParseCommits(IEnumerable<string> output)
        {
            // determine if the output is null
            if (output == null)
            {
                // break the iterator
                yield break;
            }

            // get the lines
            var lines = output.ToList();

            // create the regex's
            var header = string.IsNullOrEmpty(this.HeaderPattern) ? null : new Regex(this.HeaderPattern);
            var revert = string.IsNullOrEmpty(this.RevertPattern) ? null : new Regex(this.RevertPattern);
            var merge = string.IsNullOrEmpty(this.MergePattern) ? null : new Regex(this.MergePattern);
            var field = string.IsNullOrEmpty(this.FieldPattern) ? null : new Regex(this.FieldPattern);

            // define the match variable used to track regex matches
            var match = default(Match);

            // iterate over each line of output
            for (var i = 0; i < lines.Count; i++)
            {
                // get the hash
                var hash = lines[i++];

                // get the abbreviated hash
                var spec = lines[i++];

                // get the subject
                var subject = lines[i++];

                // create an empty string to retain the body
                var body = string.Empty;

                // get the next line
                var line = lines[i++];

                // continue processing until the split marker
                while (!Split.Equals(line))
                {
                    body += body.Length > 0 ? $"{Environment.NewLine}{line}" : line;
                    line = lines[i++];
                }

                // tricky: decrement by one since the for loop will once again skip
                // over this value
                i--;

                // create the commit
                var commit = new TaskItem(spec);
                commit.SetMetadata("Hash", $"{hash}");
                commit.SetMetadata("Message", $"{subject}{Environment.NewLine}{body}");
                commit.SetMetadata("Body", body);

                // tricky: subject may be overwritten below, which is why we set the header metadata to retain the
                // full header before splicing
                commit.SetMetadata("Header", subject);
                commit.SetMetadata("Subject", subject);

                // determine if a header regex exists
                if (header != null)
                {
                    // get the match
                    match = header.Match(subject);

                    // get the total matches
                    // tricky: match[0] will always be the entire string in .net, which we ignore
                    var matches = match.Groups.Count + 1;

                    // iterate over each header correspondence
                    for (var j = 0; j < this.HeaderCorrespondence.Length && j < matches; j++)
                    {
                        // get the associated match group
                        var group = match.Groups[j+1];

                        // get the correspondence metadata
                        var correspondence = this.HeaderCorrespondence[j];

                        // get the name of the metadata
                        var name = correspondence.GetMetadata("Name") ?? correspondence.ItemSpec;

                        // get the value of the match
                        var value = group.Value;

                        // set the metadata
                        commit.SetMetadata(name, value);
                    }
                }

                // yield the commit
                yield return commit;
            }
        }

        /// <summary>
        /// Creates a Microsoft Build execution task used to execute a git command.
        /// </summary>
        /// <param name="command">
        /// The git command that should be executed.
        /// </param>
        /// <param name="root">
        /// The root path in which to execute the git command.
        /// </param>
        /// <returns>
        /// The execution task that can be used to execute the git command.
        /// </returns>
        private Exec CreateExecTask(string command, string root)
        {
            // determine if we are on windows
            if (IsWinblows)
            {
                // escape the '%' to ensure that the temporary response file created by Exec doesn't
                // attempt to parse environment variables in format string
                command = command.Replace("%", "%%");
            }

            // create a new exec
            return new QuietExec
            {
                Command = Invariant($"git {command}"),
                WorkingDirectory = root,
                BuildEngine = this.BuildEngine,
                ConsoleToMSBuild = true,
                UseCommandProcessor = false,
                IgnoreExitCode = true,
                EchoOff = true
            };
        }
        #endregion
    }
}