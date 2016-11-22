namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the properties and methods required to implement a git log parser.
    /// </summary>
    public interface IGitLogParser
    {
        #region Methods
        /// <summary>
        /// Parses the specified lines using the specified <paramref name="options"/>
        /// </summary>
        /// <param name="lines">
        /// The lines emitted from the git log command.
        /// </param>
        /// <param name="options">
        /// The options used to parse the log.
        /// </param>
        /// <returns>
        /// The parsed git log.
        /// </returns>
        GitLog Parse(IList<string> lines, IGitLogOptions options);
        #endregion
    }
}