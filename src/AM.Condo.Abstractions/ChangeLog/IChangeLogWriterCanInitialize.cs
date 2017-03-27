// <copyright file="IChangeLogWriterCanInitialize.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.ChangeLog
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines the properties and methods required to implement a change log writer that has not yet been initialized.
    /// </summary>
    public interface IChangeLogWriterCanInitialize
    {
        #region Methods
        /// <summary>
        /// Initializes the change log at the specified <paramref name="path"/>
        /// using the specified initial <paramref name="content"/>.
        /// </summary>
        /// <param name="path">
        /// The path in which to initialize the change log.
        /// </param>
        /// <param name="content">
        /// The text used to initialize the change log.
        /// </param>
        /// <returns>
        /// The current change log writer.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the specified <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified <paramref name="path"/> is empty.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if the specified <paramref name="path"/> is invalid, or has invalid permissions.
        /// </exception>
        IChangeLogWriterCanCompile Initialize(string path, string content);
        #endregion
    }
}
