namespace PulseBridge.Condo.Tasks
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using PulseBridge.Condo.IO;

    /// <summary>
    /// Represents a Microsoft Build task that sets a tag for the current commit with git.
    /// </summary>
    public class SetGitTag : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the tag that should be created with git.
        /// </summary>
        [Required]
        [Output]
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the remote that should be used to push the tag.
        /// </summary>
        /// <returns></returns>
        [Output]
        public string Remote { get; set; } = "origin";

        /// <summary>
        /// Gets or sets the annotation for the tag. If no annotation is specified, the annotation will be set to the
        /// tag value, which includes additional data about who created the tag and when.
        /// </summary>
        [Output]
        public string Annotation { get; set; }
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

            // determine if the tag is set
            if (string.IsNullOrEmpty(this.Tag))
            {
                // log the error
                this.Log.LogError("The tag must be set to a non-empty value.");

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

            // determine if the annotation is not set
            if (string.IsNullOrEmpty(this.Annotation))
            {
                // set the annotation to the tag
                this.Annotation = this.Tag;
            }

            // replace whitespace characters in the tag
            this.Tag = this.Tag.Replace(' ', '-').Replace(Environment.NewLine, "-");

            // create a new git repository factory
            var factory = new GitRepositoryFactory();

            try
            {
                // load the repository
                var repository = factory.Load(root);

                // set the repository tag
                repository.Tag(this.Tag);

                // push the tags
                repository.Push(this.Remote, tags: true);
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
