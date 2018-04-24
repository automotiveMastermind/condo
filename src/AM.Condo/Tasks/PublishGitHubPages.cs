// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublishGitHubPages.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System.IO;

    using AM.Condo.Diagnostics;
    using AM.Condo.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build (MSBuild) task used to publish documentation to GitHub Pages.
    /// </summary>
    public class PublishGitHubPages : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the source location of the documentation to publish to GitHub Pages.
        /// </summary>
        [Required]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the repository URI of the repository to which to publish GitHub Pages.
        /// </summary>
        [Required]
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the author name used for git commits.
        /// </summary>
        public string AuthorName { get; set; } = "condo";

        /// <summary>
        /// Gets or sets the author email used for git commits.
        /// </summary>
        public string AuthorEmail { get; set; } = "open@automotivemastermind.com";

        /// <summary>
        /// Gets or sets the branch to which to publish GitHub Pages.
        /// </summary>
        public string Branch { get; set; } = "gh-pages";

        /// <summary>
        /// Gets or sets the commit message used to commit the documentation.
        /// </summary>
        public string CommitMessage { get; set; } = "docs: updating github pages";
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Execute()
        {
            // determine if the directory exists
            if (!Directory.Exists(this.Source))
            {
                // log a warning
                this.Log.LogError
                    ($"The specified source ({this.Source}) does not exist. Documentation could not be published to GitHub Pages.");

                // move on immediately
                return false;
            }

            // create a repository factory
            var factory = new GitRepositoryFactory();

            // define a variable to retain the authorization header
            var authorization = default(string);

            // create a logger
            var logger = new CondoMSBuildLogger(this.Log);

            // load the repository
            using (var origin = factory.Load(this.RepositoryRoot, logger))
            {
                // get the authorization
                authorization = origin.Authorization;
            }

            // clone the repository to a temporary path
            using (var repository = factory.Clone(this.RepositoryUri, authorization, logger))
            {
                // set the username and email
                repository.Username = this.AuthorName;
                repository.Email = this.AuthorEmail;

                // checkout the doc branch and remove all existing files
                repository.Checkout(this.Branch).Remove(".", recursive: true);

                // recursively copy the files to the temporary repo
                new DirectoryInfo(this.Source).CopyTo(repository.RepositoryPath, overwrite: true, recursive: true);

                // add the files, commit them, and push to the branch
                repository.Add().Commit(this.CommitMessage).Push(tags: false);
            }

            // move on immediately
            return true;
        }
        #endregion
    }
}
