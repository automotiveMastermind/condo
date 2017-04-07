// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetRepositoryInfo.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;

    using AM.Condo.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about a repository.
    /// </summary>
    public class GetRepositoryInfo : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the URI of the repository that is identified by the source control server.
        /// </summary>
        [Output]
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the branch used to build the repository.
        /// </summary>
        [Output]
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the commit hash or checkin number used to build the repository.
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
        public bool HasGit { get; private set; }
        #endregion

        #region Methods
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
        public static string GetRoot(string root)
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
        public static string GetRoot(DirectoryInfo root)
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

            // attempt to use the command line first
            var result = this.TryCommandLine(root);

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
        /// <param name="root">
        /// The path to the root of the repository.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the repository information could be retrieved using the git command line
        /// tool.
        /// </returns>
        public bool TryCommandLine(string root)
        {
            var factory = new GitRepositoryFactory();

            try
            {
                // load the repository
                var repository = factory.Load(root);

                // set the client version
                this.ClientVersion = repository.ClientVersion?.ToString();

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
                this.Log.LogWarningFromException(netEx);

                // move on immediately
                return false;
            }

            // we were successful
            return true;
        }
        #endregion
    }
}
