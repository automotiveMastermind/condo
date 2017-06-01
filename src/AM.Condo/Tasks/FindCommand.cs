// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindCommand.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;

    using AM.Condo.Diagnostics;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents an Microsoft Build task used to find a command on the current path.
    /// </summary>
    public class FindCommand : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the name of the command to find.
        /// </summary>
        [Required]
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the working directory for the find command operation.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets the path of the command.
        /// </summary>
        [Output]
        public string Path { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the command exists.
        /// </summary>
        [Output]
        public bool Exists { get; private set; }
        #endregion

        /// <inheritdoc />
        public override bool Execute()
        {
            // determine if the working directory was specified
            if (string.IsNullOrEmpty(this.WorkingDirectory))
            {
                // set the working directory to the current directory
                this.WorkingDirectory = Directory.GetCurrentDirectory();
            }

            // determine if we are running on windows
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            // set the find command appropriately
            var cmd = isWindows ? "where" : "which";

            try
            {
                // create a process invoker
                var invoker = new ProcessInvoker(this.WorkingDirectory, cmd);

                // execute the invoker
                var output = invoker.Execute(this.Command, throwOnError: false);

                // determine if the command was located
                this.Exists = output.Success;

                // if the command was located
                if (this.Exists)
                {
                    // set the path of the command
                    this.Path = output.Output.First();
                }
            }
            catch (Exception netEx)
            {
                // log the exception as a warning
                this.Log.LogWarningFromException(netEx, showStackTrace: true);
            }

            // assume success
            return true;
        }
    }
}
