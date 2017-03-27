// <copyright file="IChangeLogWriterCompiled.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.ChangeLog
{
    using System;

    using AM.Condo.IO;

    /// <summary>
    /// Defines the properties and methods required to implement a change log writer that has already compiled
    /// the template.
    /// </summary>
    public interface IChangeLogWriterCompiled
    {
        #region Methods
        /// <summary>
        /// Applies the template to the specified <paramref name="log"/>.
        /// </summary>
        /// <param name="log">
        /// The git log used to apply the template.
        /// </param>
        /// <returns>
        /// The current change log writer instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the specified <paramref name="log"/> is <see langword="null"/>.
        /// </exception>
        IChangeLogWriterApplied Apply(GitLog log);
        #endregion
    }
}
