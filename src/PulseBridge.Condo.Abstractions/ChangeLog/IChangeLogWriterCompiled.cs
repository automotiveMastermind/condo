namespace PulseBridge.Condo.ChangeLog
{
    using System;

    using PulseBridge.Condo.IO;

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
