namespace PulseBridge.Condo.ChangeLog
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines the properties and methods required to implement a change log writer that does not have an associated
    /// template.
    /// </summary>
    public interface IChangeLogWriterCanCompile
    {
        #region Methods
        /// <summary>
        /// Compiles the specified <paramref name="template"/>.
        /// </summary>
        /// <param name="template">
        /// The handlebars template used to generate the change log.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the specified <paramref name="template"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified <paramref name="template"/> is empty.
        /// </exception>
        IChangeLogWriterCompiled Template(string template);

        /// <summary>
        /// Loads and compiles a template from the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path containing a handlebars template used to generate the change log.
        /// </param>
        /// <returns>
        /// The current change log writer after loading and compiling the template from the specified
        /// <paramref name="path"/>.
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
        IChangeLogWriterCompiled Load(string path);
        #endregion
    }
}
