namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using PulseBridge.Condo.IO;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about the commits within a repository.
    /// </summary>
    public class GetCommitInfo : Task
    {
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
        /// </item>
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
            // attempt to get the repository root (walking the parent until we find it)
            var root = GetRepositoryInfo.GetRoot(this.RepositoryRoot);

            // determine if the root could be found
            if (string.IsNullOrEmpty(root))
            {
                // move on immediately
                return true;
            }

            // update the repository root
            this.RepositoryRoot = root;

            try
            {
                // get the commits
                this.Commits = this.GetCommits().ToArray();
            }
            catch (Exception netEx)
            {
                // log a warning
                Log.LogWarning(netEx.Message);

                // move on immediately
                return false;
            }

            // we were successful
            return true;
        }

        private IEnumerable<ITaskItem> GetCommits()
        {
            var factory = new GitRepositoryFactory();

            // load the repository
            var repository = factory.Load(this.RepositoryRoot);

            // set the client version
            this.ClientVersion = repository.ClientVersion;

            // get the options and parser
            var options = new AngularGitLogOptions();
            var parser = new GitLogParser();

            // get the commits
            var log = repository.Log(this.From, this.To, options, parser);

            // iterate over each commit
            foreach (var commit in log.Commits)
            {
                // create a new task item
                var item = new TaskItem(commit.ShortHash);

                // set well-known metadata
                item.SetMetadata(nameof(commit.Hash), commit.Hash);
                item.SetMetadata(nameof(commit.ShortHash), commit.ShortHash);
                item.SetMetadata(nameof(commit.Header), commit.Header);
                item.SetMetadata(nameof(commit.Body), commit.Body);
                item.SetMetadata(nameof(commit.Raw), commit.Raw);
                item.SetMetadata(nameof(commit.Branches), string.Join(";", commit.Branches));
                item.SetMetadata(nameof(commit.Tags), string.Join(";", commit.Tags));
                item.SetMetadata(nameof(commit.References), string.Join(";", commit.References.Select(reference => reference.Raw)));

                // return the item
                yield return item;
            }
        }
        #endregion
    }
}