namespace PulseBridge.Condo.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="ILogger"/> interface.
    /// </summary>
    public static class LoggerExtensions
    {
        #region Methods
        /// <summary>
        /// Logs the specified <paramref name="warning"/>.
        /// </summary>
        /// <param name="logger">
        /// The logger to use to log the warning.
        /// </param>
        /// <param name="warning">
        /// The warning to log.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the specified <paramref name="warning"/> is <see langword="null"/>.
        /// </exception>
        public static void LogWarning(this ILogger logger, IEnumerable<string> warning)
        {
            if (warning == null || !warning.Any())
            {
                return;
            }

            var content = string.Join(Environment.NewLine, warning);

            logger.LogWarning(content);
        }

        /// <summary>
        /// Logs the specified <paramref name="error"/>.
        /// </summary>
        /// <param name="logger">
        /// The logger to use to log the warning.
        /// </param>
        /// <param name="error">
        /// The warning to log.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the specified <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        public static void LogError(this ILogger logger, IEnumerable<string> error)
        {
            if (error == null || !error.Any())
            {
                return;
            }

            var content = string.Join(Environment.NewLine, error);

            logger.LogError(content);
        }

        /// <summary>
        /// Logs the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="logger">
        /// The logger to use to log the warning.
        /// </param>
        /// <param name="message">
        /// The warning to log.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the specified <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public static void LogMessage(this ILogger logger, IEnumerable<string> message)
        {
            if (message == null || !message.Any())
            {
                return;
            }

            var content = string.Join(Environment.NewLine, message);

            logger.LogMessage(content);
        }
        #endregion
    }
}
