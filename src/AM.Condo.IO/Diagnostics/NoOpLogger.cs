// <copyright file="NoOpLogger.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Diagnostics
{
    using System;

    /// <summary>
    /// Represents a logger that does nothing.
    /// </summary>
    public class NoOpLogger : ILogger
    {
        #region Methods
        /// <inheritdoc />
        public void LogError(Exception exception)
        {
        }

        /// <inheritdoc />
        public void LogError(string error)
        {
        }

        /// <inheritdoc />
        public void LogMessage(string message, LogLevel level)
        {
        }

        /// <inheritdoc />
        public void LogWarning(string warning)
        {
        }
        #endregion
    }
}
