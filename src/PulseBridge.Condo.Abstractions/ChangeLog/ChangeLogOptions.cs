namespace PulseBridge.Condo.ChangeLog
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the configuration options for the change log writer.
    /// </summary>
    public class ChangeLogOptions
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets a value indicating whether or not to link references.
        /// </summary>
        public bool LinkReferences => !string.IsNullOrEmpty(this.Repository) || !string.IsNullOrEmpty(this.RepositoryUri);

        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets the owner of the repository.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets the repository URI.
        /// </summary>
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets or sets the name used to describe commits.
        /// </summary>
        public string CommitName { get; set; } = "commits";

        /// <summary>
        /// Gets or sets the name used to describe issues.
        /// </summary>
        public string IssueName { get; set; } = "issues";

        /// <summary>
        /// Gets or sets the header correspondence used to group the commits.
        /// </summary>
        public string GroupBy { get; set; } = "Type";

        /// <summary>
        /// Gets or sets the header correspondence used to sort the commits.
        /// </summary>
        public string[] SortBy { get; set; } = new[] { "Scope", "Subject" };

        /// <summary>
        /// Gets the dictionary of change log types to include in the change log.
        /// </summary>
        public IDictionary<string, string> ChangeLogTypes { get; } = new Dictionary<string, string>
        {
            { "feat", "Features" },
            { "fix", "Bug Fixes" },
            { "perf", "Performance Improvements" },
            { "revert", "Reverts" }
        };
        #endregion
    }
}
