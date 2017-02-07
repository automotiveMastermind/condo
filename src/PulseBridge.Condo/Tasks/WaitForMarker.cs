// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForMarker.cs" company="PulseBridge, Inc.">
//   © PulseBridge, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Threading;

    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build (MSBuild) task that waits for a file to be written at a specific file path before
    /// continuing.
    /// </summary>
    public class WaitForFile : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the path of the file used to mark completion.
        /// </summary>
        public string FilePath { get; set; } = ".ready";
        #endregion

        #region Methods
        /// <summary>
        /// Waits for a debugger to attach prior to continuing execution.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task has completed successfully.
        /// </returns>
        public override bool Execute()
        {
            // determine if the file path is null
            if (this.FilePath == null)
            {
                // throw an argument null exception
                throw new ArgumentNullException(nameof(FilePath));
            }

            // determine if the file length is zero
            if (this.FilePath.Length == 0)
            {
                // throw a new argument exception
                throw new ArgumentException($"The {nameof(FilePath)} cannot be empty.", nameof(FilePath));
            }

            // wait for the file to be written
            while (!File.Exists(this.FilePath))
            {
                // sleep for 5 seconds
                Thread.Sleep(5000);
            }

            // move on immediately
            return true;
        }
        #endregion
    }
}
