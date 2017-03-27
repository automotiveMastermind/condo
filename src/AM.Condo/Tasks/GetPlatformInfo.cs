// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPlatformInfo.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System.Runtime.InteropServices;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about the current platform.
    /// </summary>
    public class GetPlatformInfo : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets a value indicating whether or not the platform is a distribution of Linux.
        /// </summary>
        [Output]
        public bool IsLinux { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the platform is macOS.
        /// </summary>
        [Output]
        public bool IsMacOS { get; private set; }

        /// <summary>
        /// Gets the name of the operating system platform.
        /// </summary>
        [Output]
        public string Platform { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the platform is Windows.
        /// </summary>
        [Output]
        public bool IsWindows { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetPlatformInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // detect the windows platform
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.Platform = "Windows";
                return this.IsWindows = true;
            }

            // detect the macos platform
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                this.Platform = "macOS";
                return this.IsMacOS = true;
            }

            // detect the linux platform
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                this.Platform = "Linux";
                return this.IsLinux = true;
            }

            // write an error message to the log
            this.Log.LogError("Could not retrieve platform information for the build.");

            // the build has failed
            return false;
        }
        #endregion
    }
}
