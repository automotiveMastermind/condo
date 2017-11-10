// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloneRepository.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;

    using AM.Condo.Diagnostics;
    using AM.Condo.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that is used to clone a repository.
    /// </summary>
    public class CloneRepository : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the root of the repository for the clone.
        /// </summary>
        [Required]
        public string CloneRoot { get; set; }

        /// <summary>
        /// Gets or sets the remote URI of the repository for tracking the newly checked out branch.
        /// </summary>
        [Required]
        public string RemoteUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the repository after it is cloned.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the branch, tag, or commit that should be checked out.
        /// </summary>
        public string Checkout { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="CloneRepository"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // create a new git repository factory
            var factory = new GitRepositoryFactory();

            // define a variable to retain the authorization header
            var authorization = default(string);

            // create a logger
            var logger = new CondoMSBuildLogger(this.Log);

            // load the repository
            using (var origin = factory.Load(this.RepositoryRoot, logger))
            {
                // get the authorization header if it exists
                authorization = origin.Authorization;
            }

            try
            {
                // determine if the name is specified
                if (!string.IsNullOrEmpty(this.Name))
                {
                    // include the name in the path
                    this.CloneRoot = Path.Combine(this.CloneRoot, this.Name);
                }

                // determine if the directory exists
                if (!Directory.Exists(this.CloneRoot))
                {
                    // create the directory
                    Directory.CreateDirectory(this.CloneRoot);
                }

                // clone the repository using the remote with authorization
                var repository = factory.Clone(this.CloneRoot, this.RemoteUri, authorization, logger);

                // pull the remote
                repository.Pull(all: true);

                // determine if the checkout is set
                if (!string.IsNullOrEmpty(this.Checkout))
                {
                    // checkout the appropriate ref
                    repository.Checkout(this.Checkout);
                }

                // log a message
                this.Log.LogMessage
                    (
                        MessageImportance.High,
                        $"Cloned repository: {repository.OriginUri}:{repository.CurrentBranch} to {repository.RepositoryPath}."
                    );
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
