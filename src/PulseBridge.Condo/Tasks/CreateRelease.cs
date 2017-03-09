namespace PulseBridge.Condo.Tasks
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using PulseBridge.Condo.IO;
    using PulseBridge.Condo.Diagnostics;

    /// <summary>
    /// Represents a Microsoft Build task that is used to create a release.
    /// </summary>
    public class CreateRelease : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the version that is being released.
        /// </summary>
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the release message.
        /// </summary>
        [Required]
        public string ReleaseMessage { get; set; } = "chore(release):";

        /// <summary>
        /// Gets or sets the author name used for git commits.
        /// </summary>
        public string AuthorName { get; set; } = "condo";

        /// <summary>
        /// Gets or sets the author email used for git commits.
        /// </summary>
        public string AuthorEmail { get; set; } = "condo@pulsebridge";

        /// <summary>
        /// Gets or sets a value indicating whether or not to push the release to the remote.
        /// </summary>
        public bool Push { get; set; } = false;
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

                // set the username and email
                repository.Username = this.AuthorName;
                repository.Email = this.AuthorEmail;

                // create a release message
                var message = $"{this.ReleaseMessage} {this.Version} ***NO_CI***";

                // create the commit and tag the release
                repository.Add().Commit(message).Tag(this.Version);

                // log a message
                Log.LogMessage(MessageImportance.High, $"Created and tagged the release for version: {this.Version}...");

                // determine if we should push
                if (this.Push)
                {
                    // push the changes to the remote
                    repository.Push(tags: true);

                    // log a message
                    Log.LogMessage(MessageImportance.High, $"Pushed the release for version: {this.Version}...");
                }
            }
            catch (Exception netEx)
            {
                // log a warning
                Log.LogWarningFromException(netEx);

                // move on immediately
                return false;
            }

            // we were successful
            return true;
        }
        #endregion
    }
}
