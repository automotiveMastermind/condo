// <copyright file="WaitForDebugger.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Tasks
{
    using System.Diagnostics;
    using System.Threading;

    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build (MSBuild) task that waits for a debugger to attach prior to continuing execution.
    /// </summary>
    public class WaitForDebugger : Task
    {
        #region Methods
        /// <summary>
        /// Waits for a debugger to attach prior to continuing execution.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task has completed successfully.
        /// </returns>
        public override bool Execute()
        {
            if (!Debugger.IsAttached)
            {
                var process = Process.GetCurrentProcess();
                this.Log.LogWarning($"Waiting for debugger to attach to process {process.Id}: {process.ProcessName}");
            }

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(250);
            }

            return true;
        }
        #endregion
    }
}
