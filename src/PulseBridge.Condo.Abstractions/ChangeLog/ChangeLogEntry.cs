namespace PulseBridge.Condo.ChangeLog
{
    using System;
    using System.Collections.Generic;

    using NuGet.Versioning;

    /// <summary>
    /// Represents an entry within the changelog.
    /// </summary>
    public class ChangeLogEntry
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets a value indicating whether or not the change log is for a patch.
        /// </summary>
        public bool IsPatch => this.Version?.Patch > 0;

        /// <summary>
        /// Gets or sets the semantic version that should be generated.
        /// </summary>
        public SemanticVersion Version { get; set; }

        /// <summary>
        /// Gets or sets the semantic version that should be generated.
        /// </summary>
        public string VersionString => this.Version?.ToNormalizedString();

        /// <summary>
        /// Gets or sets the title of the changelog.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the date that the changelog is being generated.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date that the changelog is being generated.
        /// </summary>
        public string DateString => this.Date.ToString("yyyy-MM-dd");

        /// <summary>
        /// Gets the collection of commits associated with the change log entry.
        /// </summary>
        public IDictionary<string, IDictionary<string, string>> Groups { get; } = new SortedDictionary<string, IDictionary<string, string>>();

        /// <summary>
        /// Gets the collection of notes associated with the change log entry.
        /// </summary>
        public IDictionary<string, IDictionary<string, string>> Notes { get; } = new SortedDictionary<string, IDictionary<string, string>>();
        #endregion
    }
}
