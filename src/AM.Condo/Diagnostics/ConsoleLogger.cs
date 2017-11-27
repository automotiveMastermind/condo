// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Diagnostics
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a logger used to log messages, warnings, errors, and exceptions through the console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        #region Methods
        /// <inheritdoc />
        public void LogError(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Console.WriteLine(exception);
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

            Console.WriteLine(error);
        }

        /// <inheritdoc />
        public void LogMessage(string message, LogLevel level)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Length == 0)
            {
                throw new ArgumentException($"The {nameof(message)} argument must not be empty.", nameof(message));
            }

            Console.WriteLine($"{level}: {message}");
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

            Console.WriteLine(warning);
        }
        #endregion
    }
}
