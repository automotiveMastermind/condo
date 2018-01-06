// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetContent.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft build (MSBuild) task used to retrieve a content file from a remote source URI.
    /// </summary>
    public class GetContent : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the source URI of the content to retrieve.
        /// </summary>
        [Required]
        public string SourceUri { get; set; }

        /// <summary>
        /// Gets or sets the destination to which the content should be saved.
        /// </summary>
        [Required]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the expected content type of the content.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to replace the file if it already exists.
        /// </summary>
        public bool Replace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to continue on error.
        /// </summary>
        public bool ContinueOnError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to extract the content to the specified destination path.
        /// </summary>
        public bool Extract { get; set; }
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Executes the <see cref="GetContent"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task was successfully executed.
        /// </returns>
        public override bool Execute()
        {
            // attempt to create the uri
            if (!Uri.TryCreate(this.SourceUri, UriKind.Absolute, out Uri uri))
            {
                // log a warning
                this.Log.LogWarning($"The source URI of {this.SourceUri} was not a valid URI.");

                // return false
                return false;
            }

            // determine if the name is null or empty
            if (string.IsNullOrEmpty(this.Name))
            {
                // set the name to the last part of the uri
                this.Name = Path.GetFileName(this.SourceUri);
            }

            // get the location where the file should be downloaded
            var location = this.Extract ? Path.Combine(Path.GetTempPath(), Path.GetTempFileName()) : this.Destination;

            // determine if the directory does not exist
            if (!Directory.Exists(location))
            {
                // create the directory
                Directory.CreateDirectory(location);
            }

            // create the fully qualified path
            var path = Path.Combine(location, this.Name);

            // determine if the file already exists
            if (File.Exists(path))
            {
                // determine if we should not replace the file
                if (!this.Replace)
                {
                    // move on immediately
                    return true;
                }

                // delete the current file
                File.Delete(path);
            }

            try
            {
                // create a new client
                using (var client = new HttpClient())
                {
                    // get the response
                    var download = client.GetAsync(uri).GetAwaiter().GetResult();

                    // ensure that the status code is successful
                    download.EnsureSuccessStatusCode();

                    // read the file content as a stream
                    using (var stream = download.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        // determine if the content should be extracted
                        if (this.Extract)
                        {
                            // read the stream as a zip archive
                            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true))
                            {
                                // extract the archive to the directory
                                archive.ExtractToDirectory(this.Destination);
                            }

                            // move on immediately
                            return true;
                        }

                        // create a file
                        using (var file = File.Create(path))
                        {
                            // determine if the stream can seek
                            if (stream.CanSeek)
                            {
                                // seek to the beginning of the origin
                                stream.Seek(0, SeekOrigin.Begin);
                            }

                            // copy the stream to the file
                            stream.CopyTo(file);
                        }
                    }
                }
            }
            catch (Exception netEx)
            {
                // log the exception
                this.Log.LogWarningFromException(netEx);

                // return continue on error
                return this.ContinueOnError;
            }

            // assume success
            return true;
        }
        #endregion
    }
}
