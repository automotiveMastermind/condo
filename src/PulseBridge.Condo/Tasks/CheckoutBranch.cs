namespace PulseBridge.Condo.Tasks
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using PulseBridge.Condo.IO;
    using PulseBridge.Condo.Diagnostics;

    /// <summary>
    /// Represents a Microsoft Build task that is used to checkout a branch.
    /// </summary>
    public class CheckoutBranch : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the branch that should be checked out.
        /// </summary>
        [Required]
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the remote URI of the repository for tracking the newly checked out branch.
        /// </summary>
        public string RemoteUri { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="CheckoutBranch"/> task.
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

                // reset the repository
                repository.Checkout(this.Branch);

                // determine if the remote uri is set
                if (!string.IsNullOrEmpty(this.RemoteUri))
                {
                    // set the remote uri
                    repository.SetRemoteUrl("origin", this.RemoteUri);
                }

                // log a message
                Log.LogMessage(MessageImportance.High, $"Checked out the branch: {this.Branch}...");
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
