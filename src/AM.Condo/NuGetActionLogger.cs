// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetActionLogger.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        /// <inheritdoc />
        public void Log(LogLevel level, string data)
        {
            this.actions.Add(logger => logger.Log(level, data));
        }

        /// <inheritdoc />
        public void Log(ILogMessage message)
        {
            this.actions.Add(logger => logger.Log(message));
        }

        /// <inheritdoc />
        public Task LogAsync(LogLevel level, string data)
        {
            this.actions.Add(logger => logger.Log(level, data));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task LogAsync(ILogMessage message)
        {
            this.actions.Add(logger => logger.Log(message));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void LogDebug(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <inheritdoc />
        public void LogError(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <inheritdoc />
        public void LogInformation(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <inheritdoc />
        public void LogInformationSummary(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <inheritdoc />
        public void LogMinimal(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <inheritdoc />
        public void LogVerbose(string data)
        {
            // add an action for logging a debug message
            this.actions.Add(logger => logger.LogDebug(data));
        }

        /// <inheritdoc />
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
