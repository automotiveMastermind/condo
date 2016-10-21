namespace PulseBridge.Condo.Build
{
    using System;

    using static System.FormattableString;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using ILogger = NuGet.Common.ILogger;

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
        #endregion

        #region Methods
        /// <summary>
        /// Logs the specified <paramref name="data"/> as a debug message.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogDebug(string data)
        {
            this.log.LogMessage(MessageImportance.Low, data);
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as an error.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogError(string data)
        {
            this.log.LogError(data);
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as an error summary.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        /// <remarks>
        /// This is a no-op as Microsoft Build already provides summary information.
        /// </remarks>
        public void LogErrorSummary(string data)
        {
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as an information message.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogInformation(string data)
        {
            this.log.LogMessage(data);
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as an information summary.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        /// <remarks>
        /// This is a no-op as Microsoft Build already provides summary information.
        /// </remarks>
        public void LogInformationSummary(string data)
        {
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as a minimal message.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogMinimal(string data)
        {
            this.log.LogMessage(MessageImportance.High, data);
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as a verbose message.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogVerbose(string data)
        {
            this.log.LogMessage(MessageImportance.Low, data);
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as a warning.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogWarning(string data)
        {
            this.log.LogWarning(data);
        }
        #endregion
    }
}