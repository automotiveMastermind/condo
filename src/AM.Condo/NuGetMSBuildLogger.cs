// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetMSBuildLogger.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Common;

    using static System.FormattableString;

    using ILogger = NuGet.Common.ILogger;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Represents a NuGet logger that emits log messages to an underlying Microsoft Build log.
    /// </summary>
    public class NuGetMSBuildLogger : ILogger
    {
        #region Fields
        private readonly TaskLoggingHelper log;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetMSBuildLogger"/> class.
        /// </summary>
        /// <param name="log">
        /// The underlying log helper to which the output from the NuGet command line interface should log.
        /// </param>
        public NuGetMSBuildLogger(TaskLoggingHelper log)
        {
            // ensure that the log is specified
            if (log == null)
            {
                // throw a new argument null exception
                throw new ArgumentNullException(nameof(log), Invariant($"The {nameof(log)} parameter cannot be null."));
            }

            // set the log
            this.log = log;
        }

        /// <inheritdoc />
        public void Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Error:
                    this.log.LogError(data);
                    break;
                case LogLevel.Warning:
                    this.log.LogWarning(data);
                    break;
                default:
                    this.log.LogMessage(MessageImportance.Low, data);
                    break;
            }
        }

        /// <inheritdoc />
        public void Log(ILogMessage message)
        {
            this.Log(message.Level, message.Message);
        }

        /// <inheritdoc />
        public Task LogAsync(LogLevel level, string data)
        {
            return Task.Run(() => this.Log(level, data));
        }

        /// <inheritdoc />
        public Task LogAsync(ILogMessage message)
        {
            return Task.Run(() => this.Log(message.Level, message.Message));
        }
        #endregion

        #region Methods
        /// <inheritdoc />
        public void LogDebug(string data)
        {
            this.log.LogMessage(MessageImportance.Low, data);
        }

        /// <inheritdoc />
        public void LogError(string data)
        {
            this.log.LogError(data);
        }

        /// <inheritdoc />
        public void LogInformation(string data)
        {
            this.log.LogMessage(data);
        }

        /// <inheritdoc />
        public void LogInformationSummary(string data)
        {
        }

        /// <inheritdoc />
        public void LogMinimal(string data)
        {
            this.log.LogMessage(MessageImportance.High, data);
        }

        /// <inheritdoc />
        public void LogVerbose(string data)
        {
            this.log.LogMessage(MessageImportance.Low, data);
        }

        /// <inheritdoc />
        public void LogWarning(string data)
        {
            this.log.LogWarning(data);
        }
        #endregion
    }
}
