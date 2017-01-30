// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeLogWriterApplied.cs" company="PulseBridge, Inc.">
//   © PulseBridge, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PulseBridge.Condo.ChangeLog
{
    using System;
    using System.IO;

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

        /// <summary>
        /// Gets the git log that has been applied to the template.
        /// </summary>
        ChangeLog Log { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Saves the applied change log template to the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path to which the change log should be applied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the specified <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified <paramref name="path"/> is empty.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if the specified <paramref name="path"/> is invalid, or has invalid permissions.
        /// </exception>
        void Save(string path);
        #endregion
    }
}
