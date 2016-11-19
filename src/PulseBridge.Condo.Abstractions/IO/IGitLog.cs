namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the properties and methods required to implement a log of a git commit history.
    /// </summary>
    public interface IGitLog
    {
        #region Properties
        /// <summary>
        /// Gets the git item specification from which the log begins.
        /// </summary>
        string From { get; }

        /// <summary>
        /// Gets the git item specification to which the log ends.
        /// </summary>
        string To { get; }

        /// <summary>
        /// Gets the commits associated with the git log.
        /// </summary>
        IEnumerable<IGitCommit> Commits { get; }

        /// <summary>
        /// Gets the tags associated with the git log.
        /// </summary>
        IEnumerable<string> Tags { get; }
        #endregion
    }
}