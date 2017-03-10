namespace AM.Condo.Diagnostics
{
    using System;

    /// <summary>
    /// Defines the properties and methods required to implement a logger used to log messages throughout condo.
    /// </summary>
    public interface ILogger
    {
        #region Methods
        /// <summary>
        /// Logs the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="level">
        /// The level of the log entry.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the specified <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified <paramref name="message"/> is empty.
        /// </exception>
        void LogMessage(string message, LogLevel level);

        /// <summary>
        /// Logs the specified <paramref name="warning"/>.
        /// </summary>
        /// <param name="warning">
        /// The warning to log.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the specified <paramref name="warning"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified <paramref name="warning"/> is empty.
        /// </exception>
        void LogWarning(string warning);

        /// <summary>
        /// Logs the specified <paramref name="error"/>.
        /// </summary>
        /// <param name="error">
        /// The error to log.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the specified <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified <paramref name="error"/> is empty.
        /// </exception>
        void LogError(string error);

        /// <summary>
        /// Logs the specified <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the specified <paramref name="exception"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified <paramref name="exception"/> is empty.
        /// </exception>
        void LogError(Exception exception);
        #endregion
    }
}
