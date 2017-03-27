// <copyright file="RestoreSubmodules.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

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
    public class RestoreSubmodules : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets the version of the client used to access the repository.
        /// </summary>
        [Output]
        public string ClientVersion { get; private set; }
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

            // attempt to use the command line first
            return this.TryCommandLine(root);
        }

        /// <summary>
        /// Attempt to use the `git` command line tool to retrieve repository information.
        /// </summary>
        /// <param name="root">
        /// The root of the repository in which to restore submodules.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the repository information could be retrieved using the git command line
        /// tool.
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
                this.ClientVersion = repository.ClientVersion?.ToString();

                // restore submodules
                repository.RestoreSubmodules();
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
