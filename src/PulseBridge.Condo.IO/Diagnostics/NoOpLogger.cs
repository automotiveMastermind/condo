using System;

namespace PulseBridge.Condo.Diagnostics
{
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
        public void LogMessage(string message)
        {
        }

        /// <inheritdoc />
        public void LogWarning(string warning)
        {
        }
        #endregion
    }
}
