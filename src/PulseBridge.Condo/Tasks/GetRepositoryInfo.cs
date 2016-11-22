namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using PulseBridge.Condo.IO;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about a repository.
    /// </summary>
    public class GetRepositoryInfo : Task
    {
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
        /// Gets the version of the client used to access the repository.
        /// </summary>
        [Output]
        public string ClientVersion { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the repository supports git.
        /// </summary>
        [Output]
        public bool HasGit { get; set; }
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

            // set the has git flag
            this.HasGit = true;

            // attempt to use the command line first, then the file system to lookup repository info
            var result = this.TryCommandLine(root) || this.TryFileSystem(root);

            // determine if we were successful and the repository url ends with a ".git" extension
            if (this.RepositoryUri != null && this.RepositoryUri.EndsWith(".git"))
            {
                // strip the '.git' from the uri
                // tricky: this is done to support browsing to the repository from
                // a browser rather than just cloning directly for github
                this.RepositoryUri.Substring(0, this.RepositoryUri.Length - 4);
            }

            // determine if the branch is set and starts with /refs/heads
            if (this.Branch != null && this.Branch.ToLower().StartsWith("refs/heads/"))
            {
                // remove the /refs/heads reference from the branch name
                this.Branch = this.Branch.Substring(11);
            }

            // return the result
            return result;
        }

        /// <summary>
        /// Attempt to use the `git` command line tool to retrieve repository information.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the repository information could be retrieved using the git command line tool.
        /// </returns>
        public bool TryCommandLine(string root)
        {
            // determine if the root is specified
            if (root == null)
            {
                // set the root
                root = this.RepositoryRoot;
            }

            var factory = new GitRepositoryFactory();

            try
            {
                // load the repository
                var repository = factory.Load(root);

                // set the client version
                this.ClientVersion = repository.ClientVersion;

                // determine if the repository uri is not already set
                if (string.IsNullOrEmpty(this.RepositoryUri))
                {
                    // set the repository uri
                    this.RepositoryUri = repository.OriginUri;
                }

                // determine if the branch is not already set
                if (string.IsNullOrEmpty(this.Branch))
                {
                    // set the branch
                    this.Branch = repository.CurrentBranch;
                }

                // determine if the commit is not already set
                if (string.IsNullOrEmpty(this.CommitId))
                {
                    // set the current commit id
                    this.CommitId = repository.LatestCommit;
                }
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

        /// <summary>
        /// Attempt to read the git configuration from the file system to retrieve repository information.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the repository information could be retrieved using the file system.
        /// </returns>
        public bool TryFileSystem(string root)
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
                        }
                    }
                }
            }

            // success
            return true;
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
        /// <paramref name="root"/> path and walking upward.
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
        /// <paramref name="root"/> path and walking upward.
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

            // determine if the directory exists
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