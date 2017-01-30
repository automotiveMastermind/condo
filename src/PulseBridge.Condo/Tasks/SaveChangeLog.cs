namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using PulseBridge.Condo.ChangeLog;

    /// <summary>
    /// Represents a Microsoft Build task used to save an automatically generated changelog to the repository root.
    /// </summary>
    public class SaveChangeLog : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the path of the repository root.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the collection of partials associated with the changelog.
        /// </summary>
        [Required]
        public ITaskItem[] Partials { get; set; } = new[]
        {
            new TaskItem("Targets/Versioning/Angular/header.hbs"),
            new TaskItem("Targets/Versioning/Angular/footer.hbs"),
            new TaskItem("Targets/Versioning/Angular/commit.hbs")
        };

        /// <summary>
        /// Gets or sets the template used to generate the changelog.
        /// </summary>
        [Required]
        public string Template { get; set; } = "Targets/Versioning/Angular/template.hbs";

        /// <summary>
        /// Gets or sets the name of the changelog.
        /// </summary>
        public string Name { get; set; } = "changelog.md";
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="SaveChangeLog"/> task.
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

            if (string.IsNullOrEmpty(this.Name))
            {
                // log the error
                this.Log.LogError("The name of the changelog must be specified.");

                // move on immediately
                return false;
            }

            var path = Path.Combine(this.RepositoryRoot, this.Name);

            try
            {
                var writer = new ChangeLogWriter();

                foreach (var partial in this.Partials)
                {
                    writer.LoadPartial(partial.ItemSpec);
                }

                writer.Load(this.Template)
                    .Apply(GetCommitInfo.GitLog)
                    .Save(path);
            }
            catch (Exception netEx)
            {
                Log.LogErrorFromException(netEx);

                return false;
            }

            // assume we were successful
            return true;
        }
        #endregion
    }
}
