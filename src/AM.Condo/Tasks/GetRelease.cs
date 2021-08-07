// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetRelease.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using AM.Condo.Resources;

    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;
    using ICSharpCode.SharpZipLib.Zip;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a Microsoft Build task used to locate and acquire the latest release of a GitHub project.
    /// </summary>
    public class GetRelease : Task
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
        /// Gets or sets the tag of the release to retrieve.
        /// </summary>
        [Required]
        public string Tag { get; set; } = "latest";

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

        /// <summary>
        /// Gets or sets a value indicating whether or not to extract the retrieved asset.
        /// </summary>
        public bool Extract { get; set; } = true;
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
            var path = $"repos/{this.Organization}/{this.Repository}/releases/";

            // append the tag
            path += this.Tag.Equals("latest", StringComparison.OrdinalIgnoreCase) ? this.Tag : $"tags/{this.Tag}";

            // attempt to create the repository uri
            if (!Uri.TryCreate(baseUri, path, out Uri uri))
            {
                // log the error
                this.Log.LogError($"The organization ({this.Organization}), repository ({this.Repository}), or tag ({this.Tag}) is invalid.");

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

                    // create the version tag
                    var version = $".version-{value.Id}-{asset.Id}";

                    // create the version path
                    var versionPath = Path.Combine(this.Destination, version);

                    // determine if the file exists
                    if (File.Exists(versionPath))
                    {
                        // move on immediately
                        return true;
                    }

                    // set the asset name
                    this.Asset = asset.Name;

                    // ensure the uri is valid
                    if (!Uri.TryCreate(asset.BrowserDownloadUrl, UriKind.Absolute, out var assetUri))
                    {
                        this.Log.LogError($@"Failed to get release:
                            the asset download uri is not valid ({asset.BrowserDownloadUrl})");

                        return false;
                    }

                    // get the file
                    var download = client.GetAsync(assetUri).GetAwaiter().GetResult();

                    // ensure that the status code is successful
                    download.EnsureSuccessStatusCode();

                    // read the file content as a stream
                    using (var stream = download.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        // determine if we need to extract the assets
                        if (this.Extract)
                        {
                            // extract the asset archive
                            this.ExtractArchive(stream);
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

        private void ExtractArchive(Stream stream)
        {
            // determine if the asset is a tar + gzip file
            if (this.Asset.EndsWith(".tar.gz", StringComparison.InvariantCultureIgnoreCase))
            {
                // extract the files
                this.ExtractTarGzip(stream);

                // move on immediately
                return;
            }

            if (this.Asset.EndsWith(".gz", StringComparison.InvariantCultureIgnoreCase))
            {
                // extract the file
                this.ExtractGzip(stream);

                // move on immediately
                return;
            }

            // determine if the asset is a tar file
            if (this.Asset.EndsWith(".tar", StringComparison.InvariantCultureIgnoreCase))
            {
                // extract the tarball
                this.ExtractTar(stream);

                // move on immediately
                return;
            }

            // determine if the asset is a zip file
            if (this.Asset.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                // extract the zip
                this.ExtractZip(stream);

                // move on immediately
                return;
            }
        }

        private void ExtractZip(Stream stream)
        {
            // extract the zip to the target directory
            new FastZip().ExtractZip
            (
                stream,
                this.Destination,
                FastZip.Overwrite.Always,
                confirmDelegate: null,
                fileFilter: null,
                directoryFilter: null,
                restoreDateTime: true,
                isStreamOwner: false,
                allowParentTraversal: false
            );
        }

        private void ExtractTarGzip(Stream stream)
        {
            // get the stream as a gzip stream
            using (var gzip = new GZipInputStream(stream))
            {
                // extract this tarball from the gzip stream
                this.ExtractTar(gzip);
            }
        }

        private void ExtractTar(Stream stream)
        {
            // get the tar archive
            using (var archive = TarArchive.CreateInputTarArchive(stream, Encoding.UTF8))
            {
                // extract the archive
                archive.ExtractContents(this.Destination);
            }
        }

        private void ExtractGzip(Stream stream)
        {
            // get the stream as a gzip stream
            using (var gzip = new GZipInputStream(stream))
            {
                // get the path for the output file
                var path = Path.Combine(this.Destination, Path.GetFileNameWithoutExtension(this.Asset));

                // create a stream for the file
                using (var file = File.Create(path))
                {
                    // copy the gzip stream to the file stream
                    gzip.CopyTo(file);
                }
            }
        }
        #endregion
    }
}
