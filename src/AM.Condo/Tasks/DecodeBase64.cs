// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecodeBase64.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents an MSBuild task used to decode a base64 string.
    /// </summary>
    public class DecodeBase64 : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the text that should be decoded.
        /// </summary>
        [Required]
        public string Encoded { get; set; }

        /// <summary>
        /// Gets or sets the text that has been decoded.
        /// </summary>
        [Output]
        public string Decoded { get; set; }

        /// <summary>
        /// Gets or sets the encoding to use to decode the text.
        /// </summary>
        public string EncodingName { get; set; } = "UTF-8";
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Execute()
        {
            try
            {
                // get the encoding
                var encoding = System.Text.Encoding.GetEncoding(this.EncodingName ?? "UTF-8");

                // get the bytes for the base64 string
                var data = Convert.FromBase64String(this.Encoded);

                // set the text accordingly
                this.Decoded = encoding.GetString(data)
                    .Replace("\n", string.Empty)
                    .Replace("\r", string.Empty);
            }
            catch (Exception netEx)
            {
                // log the exception
                this.Log.LogErrorFromException(netEx);

                // assume failure
                return false;
            }

            // assume success
            return true;
        }
        #endregion
    }
}
