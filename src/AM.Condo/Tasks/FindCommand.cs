// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindCommand.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
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
        /// Gets the search paths to use when locating the command.
        /// </summary>
        public ITaskItem[] SearchPaths { get; }

        /// <summary>
        /// Gets the path of the command.
        /// </summary>
        [Output]
        public string ExecutablePath { get; private set; }

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

            // create a list for extensions
            var extensions = new List<string>();

            // create a command for search
            var cmd = "which";

            // determine if we're running on windows
            if (isWindows)
            {
                // add common windows extensions
                extensions.Add("exe");
                extensions.Add("cmd");
                extensions.Add("bat");

                // set the cmd to where windows
                cmd = "where";
            }

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
                    this.ExecutablePath = output.Output.First();

                    // move on immediately
                    return true;
                }

                // determine if no more search paths exist
                if (this.SearchPaths == null || this.SearchPaths.Length == 0)
                {
                    // move on immediately
                    return true;
                }

                // iterate over each search path
                foreach (var path in this.SearchPaths.Select(item => item.ItemSpec))
                {
                    // create the command path
                    cmd = Path.Combine(path, this.Command);

                    // attempt to find the command
                    if (this.Exists = File.Exists(cmd))
                    {
                        // break if found
                        break;
                    }

                    // iterate over extensions
                    foreach (var extension in extensions)
                    {
                        // create the executable path
                        var executable = $"{cmd}.{extension}";

                        // attempt to find the command
                        if (this.Exists = File.Exists(cmd))
                        {
                            // break if found
                            break;
                        }
                    }
                }

                // if the command was located
                if (this.Exists)
                {
                    // set the path of the command
                    this.ExecutablePath = cmd;

                    // move on immediately
                    return true;
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
