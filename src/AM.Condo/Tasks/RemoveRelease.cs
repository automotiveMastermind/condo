// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveRelease.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Linq;
    using AM.Condo.Diagnostics;
    using AM.Condo.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that is used to remove a release.
    /// </summary>
    public class RemoveRelease : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the version of the release to remove.
        /// </summary>
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the prefix for version tags.
        /// </summary>
        public string VersionPrefix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to remove all prereleases.
        /// </summary>
        public bool Prerelease { get; set; } = true;
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="CreateRelease"/> task.
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
                return false;
            }

            // create a new git repository factory
            var factory = new GitRepositoryFactory();

            try
            {
                // load the repository
                var repository = factory.Load(root, new CondoMSBuildLogger(this.Log));

                // get all of the tags for the repository
                var tags = repository.Tags;

                // capture the version
                var prefix = this.Version;

                // determine if their is a prefix
                if (!string.IsNullOrEmpty(this.VersionPrefix))
                {
                    prefix = this.VersionPrefix + prefix;
                }

                // determine if a prerelease exists
                if (this.Prerelease)
                {
                    // append a pre-release marker
                    prefix += "-";
                }

                // iterate over each tag that starts with the specified prefix
                foreach (var tag in tags.Where(tag => tag.StartsWith(prefix)))
                {
                    // remove the tag
                    repository.RemoveTag(tag);
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
