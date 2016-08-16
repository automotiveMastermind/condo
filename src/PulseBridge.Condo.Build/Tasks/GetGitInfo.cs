namespace PulseBridge.Condo.Build.Tasks
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about a git repository.
    /// </summary>
    public class GetGitInfo : Task
    {
        /// <summary>
        /// Gets or sets the root of the git repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets the URI of the repository that is identified as the 'origin' remote.
        /// </summary>
        [Output]
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets the name of the branch used to build the repository.
        /// </summary>
        [Output]
        public string Branch { get; private set; }

        /// <summary>
        /// Gets the commit hash of the HEAD used to build the repository.
        /// </summary>
        [Output]
        public string Commit { get; private set; }

        /// <summary>
        /// Executes the <see cref="GetGitInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // attempt to get the repository root (walking the parent until we find it)
            var root = GetGitInfo.GetRepositoryRoot(this.RepositoryRoot);

            // determine if the root could be found
            if (string.IsNullOrEmpty(root))
            {
                // move on immediately
                return false;
            }

            // create the path to the head node marker
            var node = Path.Combine(root, ".git", "HEAD");

            // determine if the file exists
            if (!File.Exists(node))
            {
                // move on immediately
                return false;
            }

            // read the data from the head node
            var head = File.ReadAllText(node);

            // create the regular expression for matching the branch
            var match = Regex.Match(head, "^ref: (?<head>refs/heads/)(?<branch>.*)$");

            // determine if their was a match
            if (match.Success)
            {
                // get the branch
                this.Branch = match.Groups["branch"].Value;

                var path = match.Groups["head"].Value + this.Branch;

                // get the branch node marker path
                node = Path.Combine(root, ".git", path.Replace("/", Path.DirectorySeparatorChar.ToString()));

                // determine if the node exists
                if (File.Exists(node))
                {
                    // set the commit id
                    this.Commit = File.ReadAllText(node).Trim();
                }
            }
            else
            {
                // set the commit id to the data from the head
                this.Commit = head.Trim();

                // attempt to get the head of the origin remote
                node = Path.Combine(root, @".git\refs\remotes\origin");

                // attempt to find the branch
                this.Branch = this.FindBranch(node) ?? "<unknown>";
            }

            // find the git config marker
            node = Path.Combine(root, ".git", "config");

            // determine if the file exists
            if (!File.Exists(node))
            {
                // move on immediately
                return false;
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
                if (string.Equals(this.Commit, head))
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
        private static string GetRepositoryRoot(string root)
        {
            // determine if the directory exists
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            {
                // move on immediately
                return null;
            }

            // use the overload using a directory info
            return GetGitInfo.GetRepositoryRoot(new DirectoryInfo(root));
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
        private static string GetRepositoryRoot(DirectoryInfo root)
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
            return GetGitInfo.GetRepositoryRoot(root.Parent);
        }
    }
}