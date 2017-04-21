// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Debug.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Xunit
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Represents a helper class used to attach to a debugger.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Waits for the debugger if the XUNIT_DEBUG environment variable is <see langword="true"/>.
        /// </summary>
        public static void WaitForDebugger()
        {
            WaitForDebugger("XUNIT_DEBUG");
        }

        /// <summary>
        /// Waits for the debugger if the specified <paramref name="environmentVariable"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="environmentVariable">
        /// The name of the environment variable used to determine whether or not to wait for a debugger to attach.
        /// </param>
        public static void WaitForDebugger(string environmentVariable)
        {
            var debug = Environment.GetEnvironmentVariable(environmentVariable);

            if (!string.IsNullOrEmpty(debug) && bool.TryParse(debug, out bool enabled) && enabled)
            {
                Debugger.Launch();

                var process = Process.GetCurrentProcess();
                Console.WriteLine($"Waiting for debugger to attach to process {process.Id}: {process.ProcessName}");

                while (!Debugger.IsAttached)
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
