// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetDockerConfig.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Text;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents an MSBuild task used to create a docker config file.
    /// </summary>
    public class SetDockerConfig : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the username to use for docker.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password to use for docker.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the registry URI to use for docker.
        /// </summary>
        [Required]
        public string RegistryUri { get; set; }

        /// <summary>
        /// Gets or sets the output path of the config file that was generated.
        /// </summary>
        [Output]
        public string ConfigPath { get; set; }
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Execute()
        {
            try
            {
                // create the auth string
                var auth = $"{this.Username}:{this.Password}";

                // encode the auth string using UTF8
                var encoded = Encoding.UTF8.GetBytes(auth);

                // convert to base 64
                var base64 = Convert.ToBase64String(encoded);

                // create the json
                var json = $"{{ \"auths\": {{ \"{this.RegistryUri}\" : {{ \"auth\": \"{base64}\" }} }} }}";

                // create a temporary file
                var temporary = Path.GetTempPath();

                // create the temporary directory
                Directory.CreateDirectory(temporary);

                // create the config path
                this.ConfigPath = Path.Combine(temporary, "config.json");

                // create the temporary file
                File.WriteAllText(this.ConfigPath, json);

                // set the environment variable
                Environment.SetEnvironmentVariable("DOCKER_CONFIG", temporary);
            }
            catch (Exception netEx)
            {
                // write the exception to the log
                this.Log.LogErrorFromException(netEx);

                // move on immediately
                return false;
            }

            // assume success
            return true;
        }
        #endregion
    }
}
