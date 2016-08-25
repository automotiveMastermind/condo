namespace PulseBridge.Condo.Build.Tasks
{
    using System.Runtime.InteropServices;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about the current platform.
    /// </summary>
    public class GetPlatformInfo : Task
    {
        /// <summary>
        /// Gets a value indicating whether or not the platform is a distribution of Linux.
        /// </summary>
        [Output]
        public bool Linux { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the platform is macOS.
        /// </summary>
        [Output]
        public bool MacOS { get; private set; }

        /// <summary>
        /// Gets the name of the operating system platform.
        /// </summary>
        [Output]
        public string Platform { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the platform is Windows.
        /// </summary>
        [Output]
        public bool Windows { get; private set; }

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
                return this.Windows = true;
            }

            // detect the macos platform
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Platform = "macOS";
                return this.MacOS = true;
            }

            // detect the linux platform
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Platform = "Linux";
                return this.Linux = true;
            }

            // write an error message to the log
            Log.LogError("Could not retrieve platform information for the build.");

            // the build has failed
            return false;
        }
    }
}