namespace PulseBridge.Condo.IO
{
    using System;

    using NuGet.Versioning;

    /// <summary>
    /// Represents a git tag associated with an item spec (commit).
    /// </summary>
    public class GitTag : IComparable<GitTag>
    {
        private string name;
        private SemanticVersion version;

        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;

                SemanticVersion.TryParse(this.name, out version);
            }
        }

        /// <summary>
        /// Gets or sets the full hash of the commit associated with the tag.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the short hash of the tag.
        /// </summary>
        public string ShortHash { get; set; }

        /// <summary>
        /// Gets the semantic version of the tag.
        /// </summary>
        public SemanticVersion Version => this.version;

        /// <summary>
        /// Determines if the current tag is equal to, less then, or greater than the specified <paramref name="other"/>
        /// tag.
        /// </summary>
        /// <param name="other">
        /// The other tag to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the current tag is equal to (0), less than (-1), or greater than (1) the
        /// specified <paramref name="other"/> tag.
        /// </returns>
        public int CompareTo(GitTag other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this.Version != null && other.Version != null)
            {
                return this.Version.CompareTo(other.Version);
            }

            return string.Compare(this.Name, 0, other.Name, 0, int.MaxValue, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}
