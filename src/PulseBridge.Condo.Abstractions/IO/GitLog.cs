namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a log of git commits.
    /// </summary>
    public class GitLog
    {
        #region Properties
        /// <inheritdoc/>
        public string From { get; set; }

        /// <inheritdoc/>
        public string To { get; set; }

        /// <inheritdoc/>
        public ICollection<GitCommit> Commits { get; } = new HashSet<GitCommit>();

        /// <inheritdoc/>
        public IEnumerable<string> Tags { get; } = new HashSet<string>();
        #endregion
    }
}