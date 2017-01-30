namespace PulseBridge.Condo.Tasks
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using PulseBridge.Condo.IO;

    /// <summary>
    /// Represents a Microsoft Build task that pushes changes, including tags, to a remote branch.
    /// </summary>
    public class PushChanges : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the remote that should be used to push the tag.
        /// </summary>
        /// <returns></returns>
        [Output]
        public string Remote { get; set; } = "origin";
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
            if (string.IsNullOrEmpty(this.Remote))
            {
                // log the error
                this.Log.LogError("The remote must be specified.");

                // move on immediately
                return false;
            }

            // create a new git repository factory
            var factory = new GitRepositoryFactory();

            try
            {
                // load the repository
                var repository = factory.Load(root);

                // push changes to the remote repository
                repository.Add().Push(this.Remote, tags: true);

                // log a message
                Log.LogMessage(MessageImportance.High, $"Pushed changes to remote: {this.Remote}");
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
        #endregion
    }
}
