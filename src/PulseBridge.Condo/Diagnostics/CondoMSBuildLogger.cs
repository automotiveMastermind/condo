namespace PulseBridge.Condo.Diagnostics
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a logger used to log messages, warnings, errors, and exceptions through an MSBuild task.
    /// </summary>
    public class CondoMSBuildLogger : ILogger
    {
        #region Private Fields
        private readonly TaskLoggingHelper log;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="CondoMSBuildLogger"/> class.
        /// </summary>
        /// <param name="log">
        /// The task logging helper used to log messages.
        /// </param>
        public CondoMSBuildLogger(TaskLoggingHelper log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            // set the log
            this.log = log;
        }
        #endregion

        #region Methods
        /// <inheritdoc />
        public void LogError(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            this.log.LogErrorFromException(exception);
        }

        /// <inheritdoc />
        public void LogError(string error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            if (error.Length == 0)
            {
                throw new ArgumentException($"The {nameof(error)} argument must not be empty.", nameof(error));
            }

            this.log.LogError(error);
        }

        /// <inheritdoc />
        public void LogMessage(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Length == 0)
            {
                throw new ArgumentException($"The {nameof(message)} argument must not be empty.", nameof(message));
            }

            this.log.LogMessage(MessageImportance.High, message);
        }

        /// <inheritdoc />
        public void LogWarning(string warning)
        {
            if (warning == null)
            {
                throw new ArgumentNullException(nameof(warning));
            }

            if (warning.Length == 0)
            {
                throw new ArgumentException($"The {nameof(warning)} argument must not be empty.", nameof(warning));
            }

            this.log.LogWarning(warning);
        }
        #endregion
    }
}
