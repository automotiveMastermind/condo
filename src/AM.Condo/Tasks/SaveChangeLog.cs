// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveChangeLog.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;

    using AM.Condo.ChangeLog;
    using AM.Condo.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Versioning;

    /// <summary>
    /// Represents a Microsoft Build task used to save an automatically generated changelog to the repository root.
    /// </summary>
    public class SaveChangeLog : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the path of the repository root.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the collection of partials associated with the changelog.
        /// </summary>
        [Required]
        public ITaskItem[] Partials { get; set; } = new[]
        {
            new TaskItem("Targets/Version/Angular/header.hbs"),
            new TaskItem("Targets/Version/Angular/footer.hbs"),
            new TaskItem("Targets/Version/Angular/commit.hbs")
        };

        /// <summary>
        /// Gets or sets the template used to generate the changelog.
        /// </summary>
        [Required]
        public string Template { get; set; } = "Targets/Version/Angular/template.hbs";

        /// <summary>
        /// Gets or sets the types to include in the change log.
        /// </summary>
        [Required]
        public string ChangeLogTypes { get; set; }

        /// <summary>
        /// Gets or sets the display names of the change log types to include in the change log.
        /// </summary>
        [Required]
        public string ChangeLogNames { get; set; }

        /// <summary>
        /// Gets or sets the path containing the initialization template for the change log.
        /// </summary>
        [Required]
        public string ChangeLogInitialize { get; set; }

        /// <summary>
        /// Gets or sets the version for the changelog.
        /// </summary>
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the URI of the repository.
        /// </summary>
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets or sets the group by header correspondence field.
        /// </summary>
        public string GroupByHeader { get; set; }

        /// <summary>
        /// Gets or sets the sort by header correspondence fields.
        /// </summary>
        public string SortByHeader { get; set; }

        /// <summary>
        /// Gets or sets the name of the changelog.
        /// </summary>
        public string Name { get; set; } = "CHANGELOG.md";
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

            var log = GetCommitInfo.GitLog;

            if (log == null)
            {
                this.Log.LogError($"You must call the {nameof(GetCommitInfo)} task before calling this task.");

                return false;
            }

            var path = Path.Combine(this.RepositoryRoot, this.Name);

            var types = this.ChangeLogTypes.PropertySplit();
            var names = this.ChangeLogNames.PropertySplit();

            if (types.Length != names.Length)
            {
                this.Log.LogError($"The number of change log types {types.Length} must match the number of change log names {names.Length}.");

                return false;
            }

            try
            {
                var options = new ChangeLogOptions
                {
                    RepositoryUri = this.RepositoryUri,
                    GroupBy = this.GroupByHeader,
                    SortBy = this.SortByHeader.PropertySplit(),
                    Version = SemanticVersion.Parse(this.Version)
                };

                // clear the change log types
                options.ChangeLogTypes.Clear();

                // iterate over available types
                for (var i = 0; i < types.Length; i++)
                {
                    options.ChangeLogTypes.Add(types[i], names[i]);
                }

                // read the initialization of the changelog
                var content = File.ReadAllText(this.ChangeLogInitialize);

                // initialize a writer
                var writer = new ChangeLogWriter(options).Initialize(path, content);

                // iterate over each partial that is defined
                foreach (var partial in this.Partials)
                {
                    // load the partial
                    writer.LoadPartial(partial.ItemSpec);
                }

                // create a new git repository factory
                var factory = new GitRepositoryFactory();

                // save changes to the repository
                var repository = factory.Load(this.RepositoryRoot);

                // write the changelog
                writer.Load(this.Template).Apply(log).Save();

                // track changes for the changelog path
                repository.Add(this.Name, force: true);

                // write a message
                this.Log.LogMessage(MessageImportance.High, $"Saved the conventional changelog: {this.Name}...");
            }
            catch (Exception netEx)
            {
                // log an exception
                this.Log.LogErrorFromException(netEx);

                // move on immediately
                return false;
            }

            // assume we were successful
            return true;
        }
        #endregion
    }
}
