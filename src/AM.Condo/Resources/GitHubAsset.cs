// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubAsset.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Resources
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a GitHub asset.
    /// </summary>
    public class GitHubAsset
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the surrogate identifier of the asset.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the URL of the asset.
        /// </summary>
        [JsonProperty("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the asset.
        /// </summary>
        public string Name { get; set; }
        #endregion
    }
}
