namespace PulseBridge.Condo.Build.Tasks
{
    using System.Diagnostics;
    using System.Threading;

    using static System.FormattableString;

    using Microsoft.Build.Framework;
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
            var process = Process.GetCurrentProcess();
            var message = Invariant($"Waiting for debugger to attach to process {process.Id}: {process.ProcessName}");

            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            while (!Debugger.IsAttached)
            {
                this.Log.LogMessageFromText(message, MessageImportance.High);

                Thread.Sleep(250);
            }

            return true;
        }
        #endregion
    }
}