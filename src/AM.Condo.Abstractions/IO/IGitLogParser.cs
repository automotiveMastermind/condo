namespace AM.Condo.IO
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
        /// <param name="commits">
        /// The raw commit lines emitted from a git log operation.
        /// </param>
        /// <param name="options">
        /// The options used to parse the log.
        /// </param>
        /// <returns>
        /// The parsed git log.
        /// </returns>
        GitLog Parse(IEnumerable<IList<string>> commits, IGitLogOptions options);
        #endregion
    }
}