// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLatestRelease.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using AM.Condo.Resources;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a Microsoft Build task used to locate and aquire the latest release of a GitHub project.
    /// </summary>
    public class GetLatestRelease : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the name of the organization that manages the project.
        /// </summary>
        [Required]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the name of the repository for which to get the release.
        /// </summary>
        [Required]
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets the destination path where the release should be saved.
        /// </summary>
        [Required]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the name of the asset to retrieve.
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// Gets or sets the URI of the GitHub API.
        /// </summary>
        public string GitHubApi { get; set; } = "https://api.github.com";

        /// <summary>
        /// Gets or sets the authorization token used to authenticate to the GitHub API.
        /// </summary>
        public string Token { get; set; }
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Execute()
        {
            // attempt to create the root API URL
            if (!Uri.TryCreate(this.GitHubApi, UriKind.Absolute, out Uri baseUri))
            {
                // log the error
                this.Log.LogError($"The specified GitHub API URL ({this.GitHubApi}) is not valid.");

                // return false
                return false;
            }

            // create the repo path
            var path = $"repos/{this.Organization}/{this.Repository}/releases/latest";

            // attempt to create the repository uri
            if (!Uri.TryCreate(baseUri, path, out Uri uri))
            {
                // log the error
                this.Log.LogError($"The organization ({this.Organization}) or the repository ({this.Repository}) is invalid.");

                // return false
                return false;
            }

            // resolve the destination path
            this.Destination = Path.GetFullPath(this.Destination);

            try
            {
                // create a client
                using (var client = new HttpClient())
                {
                    // add the user agent header
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Request", string.Empty));

                    // add the accept header
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/zip"));

                    // issue the request
                    var response = client.GetAsync(uri).GetAwaiter().GetResult();

                    // determine if the response was successful
                    if (!response.IsSuccessStatusCode)
                    {
                        // log an error
                        this.Log.LogError
                            ($"The request was not successful. {response.StatusCode} - {response.ReasonPhrase}");

                        // return false
                        return false;
                    }

                    // attempt to parse the response
                    var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    // deserialize the object
                    var value = JsonConvert.DeserializeObject<GitHubRelease>(json);

                    // create the version tag
                    var version = $".version-{value.Id}";

                    // create the version path
                    var versionPath = Path.Combine(this.Destination, version);

                    // determine if the file exists
                    if (File.Exists(versionPath))
                    {
                        // move on immediately
                        return true;
                    }

                    // determine if the destination exists
                    if (Directory.Exists(this.Destination))
                    {
                        // delete the destination path
                        Directory.Delete(this.Destination, recursive: true);
                    }

                    // find the asset
                    var asset = string.IsNullOrEmpty(this.Asset)
                        ? value.Assets.SingleOrDefault()
                        : value.Assets.FirstOrDefault(item => item.Name.Equals(this.Asset, StringComparison.OrdinalIgnoreCase));

                    // determine if the asset is null
                    if (asset == null)
                    {
                        // log the error
                        this.Log.LogError("No asset could be found.");

                        // return false
                        return false;
                    }

                    // get the file
                    var download = client.GetAsync(asset.BrowserDownloadUrl).GetAwaiter().GetResult();

                    // ensure that the status code is successful
                    download.EnsureSuccessStatusCode();

                    // read the file content as a stream
                    using (var stream = download.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        // read the stream as a zip archive
                        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true))
                        {
                            // extract the archive to the directory
                            archive.ExtractToDirectory(this.Destination);
                        }
                    }

                    // create the version path file
                    File.Create(versionPath).Dispose();
                }
            }
            catch (Exception netEx)
            {
                // log the exception
                this.Log.LogErrorFromException(netEx);

                // return false
                return false;
            }

            // assume success
            return true;
        }
        #endregion
    }
}
