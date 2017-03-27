// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetActionLogger.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;
    using System.Collections.Generic;

    using NuGet.Common;

    /// <summary>
    /// Represents a NuGet logger that captures and temporarily retains log actions.
    /// </summary>
    public class NuGetActionLogger : ILogger
    {
        #region Fields
        private List<Action<ILogger>> actions = new List<Action<ILogger>>();
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
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as an error.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogError(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as an error summary.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogErrorSummary(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as an information message.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogInformation(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as an information summary.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogInformationSummary(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as a minimal message.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogMinimal(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as a verbose message.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogVerbose(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <summary>
        /// Logs the specified <paramref name="data"/> as a warning.
        /// </summary>
        /// <param name="data">
        /// The data that should be logged.
        /// </param>
        public void LogWarning(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <summary>
        /// Replays all of the actions that were recorded using the specified <paramre name="logger"/>.
        /// </summary>
        /// <param name="logger">
        /// The logger in which the log actions should be replayed.
        /// </param>
        public void Replay(ILogger logger)
        {
            // execute each action in order
            this.actions.ForEach(action => action(logger));
        }
        #endregion
    }
}
