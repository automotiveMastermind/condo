namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a log of git commits.
    /// </summary>
    public class GitLog : IGitLog
    {
        #region Properties
        /// <inheritdoc/>
        public string From { get; }

        /// <inheritdoc/>
        public string To { get; }

        /// <inheritdoc/>
        public IEnumerable<IGitCommit> Commits { get; }

        /// <inheritdoc/>
        public IEnumerable<string> Tags { get; }
        #endregion
    }
}