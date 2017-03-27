// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestLogger.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Diagnostics
{
    using System;
    using Xunit.Abstractions;

    public class TestLogger : ILogger
    {
        private readonly ITestOutputHelper output;

        public TestLogger(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void LogError(string error)
        {
            this.output.WriteLine($"ERROR: {error}");
        }

        public void LogError(Exception exception)
        {
            this.output.WriteLine($"EXCEPTION: {exception.Message}");
        }

        public void LogMessage(string message, LogLevel level)
        {
            this.output.WriteLine($"MESSAGE: {message} - {level}");
        }

        public void LogWarning(string warning)
        {
            this.output.WriteLine($"WARNING: {warning}");
        }
    }
}
