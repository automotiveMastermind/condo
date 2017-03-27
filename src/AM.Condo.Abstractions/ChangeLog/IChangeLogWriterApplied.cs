// <copyright file="IChangeLogWriterApplied.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.ChangeLog
{
    /// <summary>
    /// Defines the properties and methods required to implement a change log that has already been loaded and
    /// processed.
    /// </summary>
    public interface IChangeLogWriterApplied
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets the result containing the change log.
        /// </summary>
        string ChangeLog { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Saves the applied change log template.
        /// </summary>
        void Save();
        #endregion
    }
}
