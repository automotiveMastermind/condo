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
        [Output]
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
        /// Gets or sets the branch associated with the release.
        /// </summary>
        [Required]
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the URI of the repository to which to push the release.
        /// </summary>
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets or sets the author name used for git commits.
        /// </summary>
        public string AuthorName { get; set; } = "condo";

        /// <summary>
        /// Gets or sets the author email used for git commits.
        /// </summary>
        public string AuthorEmail { get; set; } = "condo@pulsebridge";
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
                return false;
            }

            // determine if the remote is set
            if (string.IsNullOrEmpty(this.RepositoryUri))
            {
                // log the error
                this.Log.LogError("The remote URI must be specified.");

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

                // determine if the repository uri is not empty
                if (!string.IsNullOrEmpty(this.RepositoryUri))
                {
                    // set the remote url
                    repository.SetRemoteUrl("origin", this.RepositoryUri);
                }

                // create a release message
                var message = $"{this.ReleaseMessage} {this.Version} ***NO_CI***";

                // push changes to the remote repository
                repository.Add().Commit(message).Push(tags: true);

                // log a message
                Log.LogMessage(MessageImportance.High, $"Pushed changes to remote: {this.RepositoryUri} @ {this.Branch}.");
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
