// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitTag.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;

    using NuGet.Versioning;

    /// <summary>
    /// Represents a git tag associated with an item spec (commit).
    /// </summary>
    public class GitTag : IComparable<GitTag>
    {
        #region Fields
        private SemanticVersion version;
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the full hash of the commit associated with the tag.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the short hash of the tag.
        /// </summary>
        public string ShortHash { get; set; }

        /// <summary>
        /// Gets the version associated with the tag.
        /// </summary>
        public SemanticVersion Version => this.version;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the version of the tag based on the specified <paramref name="versionTagPrefix"/>.
        /// </summary>
        /// <param name="versionTagPrefix">
        /// The version tag prefix used for version tags.
        /// </param>
        /// <returns>
        /// The semantic version represented by the current tag.
        /// </returns>
        public bool TryParseVersion(string versionTagPrefix)
        {
            // capture the tag name
            var tag = this.Name;

            // determine if we need to trim the version
            if (!string.IsNullOrEmpty(versionTagPrefix) && tag.StartsWith(versionTagPrefix))
            {
                // trim the version
                tag = tag.Substring(versionTagPrefix.Length);
            }

            // attempt to parse the tags
            return SemanticVersion.TryParse(tag, out this.version);
        }

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

            if (this.version != null && other.version != null)
            {
                return this.version.CompareTo(other.version);
            }

            return string.Compare(this.Name, 0, other.Name, 0, int.MaxValue, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}
