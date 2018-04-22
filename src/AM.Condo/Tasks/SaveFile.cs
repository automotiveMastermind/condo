// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFile.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Security;
    using System.Text;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task used to save a file to disk with its contents.
    /// </summary>
    public class SaveFile : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the path of the file to save.
        /// </summary>
        [Required]
        [Output]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the name of the encoding to use when saving the file.
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to replace the file if it already exists.
        /// </summary>
        public bool Replace { get; set; } = false;
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="SaveFile"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task was successful.
        /// </returns>
        public override bool Execute()
        {
            // determine if the contents are null
            if (this.Content == null)
            {
                // set the contents to an empty string
                this.Content = string.Empty;
            }

            try
            {
                // determine if the path is not rooted
                if (!Path.IsPathRooted(this.FilePath))
                {
                    // combine the file path
                    this.FilePath = Path.Combine(this.RepositoryRoot, this.FilePath);
                }

                // determine if the file already exists
                if (File.Exists(this.FilePath))
                {
                    // determine if the file can be replaced
                    if (this.Replace)
                    {
                        // delete the file
                        File.Delete(this.FilePath);
                    }
                    else
                    {
                        // write a warning
                        this.Log.LogWarning($"A file at the path: {this.FilePath} already exists and cannot be replaced.");

                        // move on immediately
                        return true;
                    }
                }

                // get the encoding specified
                var encoding = Encoding.GetEncoding(this.EncodingName ?? "UTF-8");

                // write the file content to the specified path
                File.WriteAllText(this.FilePath, this.Content, encoding);
            }
            catch (ArgumentException argEx)
            {
                // log a warning
                this.Log.LogErrorFromException(argEx);

                // move on immediately
                return false;
            }
            catch (IOException ioEx)
            {
                // log the exception
                this.Log.LogErrorFromException(ioEx);

                // move on immediately
                return false;
            }
            catch (SecurityException securityEx)
            {
                // log the exception
                this.Log.LogErrorFromException(securityEx);

                // move on immediately
                return false;
            }

            // we were successful
            return true;
        }
        #endregion
    }
}
