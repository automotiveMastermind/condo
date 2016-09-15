namespace PulseBridge.Condo.Build.Tasks
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using static System.FormattableString;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Tasks;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about a repository.
    /// </summary>
    public class GetRepositoryInfo : Task
    {
        #region Constants
        /// <summary>
        /// The command (for git) used to retrieve the git-cli version.
        /// </summary>
        private const string VersionCommand = "--version";

        /// <summary>
        /// The command (for git) used to print a log of all commits.
        /// </summary>
        private const string LogCommand = "log --decorate --no-color";

        /// <summary>
        /// The command (for git) used to print the current branch.
        /// </summary>
        private const string BranchCommand = "rev-parse --abbrev-ref HEAD";

        /// <summary>
        /// The command (for git) used to get the latest commit.
        /// </summary>
        private const string LatestCommitCommand = "rev-parse HEAD";

        /// <summary>
        /// The command (for git) used to get the first commit.
        /// </summary>
        private const string RootCommitCommand = "rev-list --max-parents=0 HEAD";

        /// <summary>
        /// The command (for git) used to get the latest tag.
        /// </summary>
        private const string LatestTagCommitCommand = "rev-list --tags --max-count=1";

        /// <summary>
        /// The command (for git) used to describe a tag.
        /// </summary>
        private const string DescribeTagCommand = "describe --tags";

        /// <summary>
        /// The command (for git) used to get the remote url of the 'origin'.
        /// </summary>
        private const string RemoteUrlCommand = "remote get-url origin";

        /// <summary>
        /// The regular expression used to discover tags within a list of commits.
        /// </summary>
        private const string TagRegex = @"tag:\s*(.+?)[,\)]";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets the URI of the repository that is identified by the source control server.
        /// </summary>
        [Output]
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets the name of the branch used to build the repository.
        /// </summary>
        [Output]
        public string Branch { get; set; }

        /// <summary>
        /// Gets the commit hash or checkin number used to build the repository.
        /// </summary>
        [Output]
        public string CommitId { get; set; }

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
        ///     The type (of change) for the commit, which may be Feature, Fix, Docs, Style, Refactor,
        ///     Test or Chore.
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
        ///     <term>Breaks</term>
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
        public ITaskItem[] NewCommits { get; set; }

        /// <summary>
        /// Gets the latest release tag contained within the repository.
        /// </summary>
        [Output]
        public string LatestTag { get; private set; }

        [Output]
        public string LatestTagCommit { get; private set; }

        /// <summary>
        /// Gest the version of the client used to access the repository.
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

            // attempt to use the command line first, then the file system to lookup repository info
            return this.TryCommandLine(root) || this.TryFileSystem(root);
        }

        /// <summary>
        /// Attempt to use the `git` command line tool to retrieve repository information.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the repository information could be retrieved using the git command line tool.
        /// </returns>
        public bool TryCommandLine(string root = null)
        {
            // determine if the root is specified
            if (root == null)
            {
                // set the root
                root = this.RepositoryRoot;
            }

            // create an exec task for retrieving the version
            var exec = this.CreateExecTask(VersionCommand, root);

            // execute the command and ensure that the output exists
            if (!exec.Execute() || exec.ConsoleOutput.Length == 0)
            {
                // move on immediately
                return false;
            }

            // set the client version
            this.ClientVersion = exec.ConsoleOutput[0].ItemSpec;

            // determine if the repository uri has not already been set
            if (string.IsNullOrEmpty(this.RepositoryUri))
            {
                // create the command for the remote uri
                exec = this.CreateExecTask(RemoteUrlCommand, root);

                // execute the command and ensure that the output contains a result
                if (exec.Execute() && exec.ConsoleOutput.Length == 1)
                {
                    // set the repository uri
                    this.RepositoryUri = exec.ConsoleOutput[0].ItemSpec;
                }
            }

            // determine if the branch has already been set
            if (string.IsNullOrEmpty(this.Branch))
            {
                // create the command for the branch
                exec = this.CreateExecTask(BranchCommand, root);

                // execute the command and ensure that the output contains a result
                if (exec.Execute() && exec.ConsoleOutput.Length == 1)
                {
                    // set the branch
                    this.Branch = exec.ConsoleOutput[0].ItemSpec;
                }
            }

            // determine if the commit id has already been set
            if (string.IsNullOrEmpty(this.CommitId))
            {
                // create the command to retrieve the commit id
                exec = this.CreateExecTask(LatestCommitCommand, root);

                // execute the command and ensure that the output contains a result
                if (exec.Execute() && exec.ConsoleOutput.Length == 1)
                {
                    // set the commit
                    this.CommitId = exec.ConsoleOutput[0].ItemSpec;
                }
            }

            // determine if the latest tag commit is empty
            if (string.IsNullOrEmpty(this.LatestTagCommit))
            {
                // create the command used to get the latest tag commit
                exec = this.CreateExecTask(LatestTagCommitCommand, root);

                // execute the command and ensure that the output contains a result
                if (exec.Execute() && exec.ConsoleOutput.Length == 1)
                {
                    // set the tag commit
                    this.LatestTagCommit = exec.ConsoleOutput[0].ItemSpec;

                    // create the command used to describe the commit
                    exec = this.CreateExecTask(Invariant($"{DescribeTagCommand} {this.LatestTagCommit}"), root);

                    // execute the command and ensure that the output contains a result
                    if (exec.Execute() && exec.ConsoleOutput.Length == 1)
                    {
                        // set the tag
                        this.LatestTag = exec.ConsoleOutput[0].ItemSpec;
                    }
                }
            }

            // we were successful
            return true;
        }

        /// <summary>
        /// Attempt to read the git configuration from the file system to retrieve repository information.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the repository information could be retrieved using the file system.
        /// </returns>
        public bool TryFileSystem(string root = null)
        {
            // determine if the root is specified
            if (root == null)
            {
                // set the root
                root = this.RepositoryRoot;
            }

            // determine if the root is now specified
            if (root == null)
            {
                // omove on immediately
                return false;
            }

            // create the path to the head node marker
            var node = Path.Combine(root, ".git", "HEAD");

            // determine if the file exists
            if (!File.Exists(node))
            {
                // move on immediately
                return true;
            }

            // read the data from the head node
            var head = File.ReadAllText(node);

            // create the regular expression for matching the branch
            var match = Regex.Match(head, "^ref: (?<branch>refs/heads/.*)$");

            // determine if their was a match
            if (match.Success)
            {
                // get the branch
                var branch = match.Groups["branch"].Value;

                // determine if the branch is not already set
                if (string.IsNullOrEmpty(this.Branch))
                {
                    // get the branch
                    this.Branch = branch;
                }

                // get the branch node marker path
                node = Path.Combine(root, ".git", branch.Replace("/", Path.DirectorySeparatorChar.ToString()));

                // determine if the commit id is not already set and the node exists
                if (string.IsNullOrEmpty(this.CommitId) && File.Exists(node))
                {
                    // set the commit id
                    this.CommitId = File.ReadAllText(node).Trim();
                }
            }
            else
            {
                // determine if the commit id is already set
                if (string.IsNullOrEmpty(this.CommitId))
                {
                    // set the commit id to the data from the head
                    this.CommitId = head.Trim();
                }

                // determine if the branch is already set
                if (string.IsNullOrEmpty(this.Branch))
                {
                    // attempt to get the head of the origin remote
                    node = Path.Combine(root, @".git\refs\remotes\origin");

                    // attempt to find the branch
                    this.Branch = this.FindBranch(node) ?? "<unknown>";
                }
            }

            // find the git config marker
            node = Path.Combine(root, ".git", "config");

            // determine if the repository uri is already set or the file exists
            if (!string.IsNullOrEmpty(this.RepositoryUri) || !File.Exists(node))
            {
                // move on immediately
                return true;
            }

            // open a file stream
            using (var file = new FileStream(node, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
            {
                // attempt to read the git config path
                using (var stream = new StreamReader(file))
                {
                    // define a variable to retain the line number
                    string line;

                    // find the line for the origin remote
                    while (((line = stream.ReadLine()) != null) && !string.Equals(line.Trim(), "[remote \"origin\"]", StringComparison.OrdinalIgnoreCase)) { }

                    // determine if the line exists
                    if (!string.IsNullOrEmpty(line))
                    {
                        // attempt to find the
                        match = Regex.Match(stream.ReadLine(), @"^\s+url\s*=\s*(?<url>[^\s]*)\s*$");

                        // determine if a match was found
                        if (match.Success)
                        {
                            // get the match for the uri
                            this.RepositoryUri = match.Groups["url"].Value;

                            // determine if the match ends with '.git'
                            if (this.RepositoryUri.EndsWith(".git"))
                            {
                                // strip the '.git' from the uri
                                // tricky: this is done to support browsing to the repository from
                                // a browser rather than just cloning directly for github
                                this.RepositoryUri.Substring(0, this.RepositoryUri.Length - 4);
                            }
                        }
                    }
                }
            }

            // success
            return true;
        }

        private Exec CreateExecTask(string command, string root)
        {
            // create a new exec
            return new QuietExec
            {
                Command = Invariant($"git {command}"),
                WorkingDirectory = root,
                BuildEngine = this.BuildEngine,
                ConsoleToMSBuild = true,
                EchoOff = true
            };
        }

        /// <summary>
        /// Attempts to find the branch that matches the expected commit.
        /// </summary>
        /// <param name="path">
        /// The path to the remote git configuration path containing markers for branches on the remote.
        /// </param>
        /// <returns>
        /// The name of the branch whose HEAD matches the expected commit.
        /// </returns>
        /// <remarks>
        /// This is a last-ditch effort to try to 'discover' the branch. If multiple
        /// branches all contain the same commit, then this could be innacurate, but they should
        /// be building the exact same code regardless.
        /// </remarks>
        private string FindBranch(string path)
        {
            // determine if the path exists
            if (!Directory.Exists(path))
            {
                // move on immediately
                return null;
            }

            // iterate over each file in the remote folder
            foreach (var branch in Directory.GetFiles(path))
            {
                // read the commit from the head
                var head = File.ReadAllText(branch).Trim();

                // see if the commit matches the head
                if (string.Equals(this.CommitId, head))
                {
                    // assume we found the right branch
                    return Path.GetFileName(branch).Trim();
                }

                // iterate over each child branch
                foreach(var part in Directory.GetDirectories(path))
                {
                    // attempt to find the branch
                    var current = this.FindBranch(part);

                    // determine if the branch was found
                    if (current != null)
                    {
                        // return the child branch
                        return Path.GetFileName(part) + '/' + current;
                    }
                }
            }

            // move on immediately
            return null;
        }

        /// <summary>
        /// Attempt to find the git repository root by scanning the specified
        /// <paramref cref="root"/> path and walking upward.
        /// </summary>
        /// <param name="root">
        /// The starting path that is believed to be the root path.
        /// </param>
        /// <returns>
        /// The path that is the repository root path, or <see langword="null"/> if no root
        /// path could be found.
        /// </returns>
        private static string GetRoot(string root)
        {
            // determine if the directory exists
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            {
                // move on immediately
                return null;
            }

            // use the overload using a directory info
            return GetRepositoryInfo.GetRoot(new DirectoryInfo(root));
        }

        /// <summary>
        /// Attempt to find the git repository root by scanning the specified
        /// <paramref cref="root"/> path and walking upward.
        /// </summary>
        /// <param name="root">
        /// The starting path that is believed to be the root path.
        /// </param>
        /// <returns>
        /// The path that is the repository root path, or <see langword="null"/> if no root
        /// path could be found.
        /// </returns>
        private static string GetRoot(DirectoryInfo root)
        {
            // determine if the starting directory exists
            if (root == null)
            {
                // move on immediately
                return null;
            }

            // create the path for the .git folder
            var path = Path.Combine(root.FullName, ".git");

            if (Directory.Exists(path))
            {
                // move on immediately
                return root.FullName;
            }

            // walk the tree to the parent
            return GetRepositoryInfo.GetRoot(root.Parent);
        }
        #endregion
    }
}