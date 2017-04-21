// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubAsset.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace AM.Condo.Resources
{
    /// <summary>
    /// Represents a GitHub release.
    /// </summary>
    public class GitHubRelease
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the surrogate identifier of the release.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the tag name of the release.
        /// </summary>
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the URL of the tarball download.
        /// </summary>
        [JsonProperty("tarball_url")]
        public string TarballUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL of the zipball download.
        /// </summary>
        [JsonProperty("zipball_url")]
        public string ZipballUrl { get; set; }

        /// <summary>
        /// Gets the collection of assets associated with the release.
        /// </summary>
        public ICollection<GitHubAsset> Assets { get; } = new HashSet<GitHubAsset>();
        #endregion
    }
}
