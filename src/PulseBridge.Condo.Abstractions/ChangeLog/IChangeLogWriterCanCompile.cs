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
        /// <returns>
        /// The current change log writer.
        /// </returns>
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

        /// <summary>
        /// Loads and compiles a partial tempalte from the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path containing a handlebars partial template used to generate the change log.
        /// </param>
        /// <returns>
        /// The current change log writer after loading and compiling the partial template from the specified
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
        IChangeLogWriterCanCompile LoadPartial(string path);

        /// <summary>
        /// Compiles the specified <paramref name="partial"/> template.
        /// </summary>
        /// <param name="name">
        /// The name of the partial template.
        /// </param>
        /// <param name="partial">
        /// The handlebars partial template used to generate the change log.
        /// </param>
        /// <returns>
        /// The current change log writer.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the specified <paramref name="partial"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified <paramref name="partial"/> is empty.
        /// </exception>
        IChangeLogWriterCanCompile Partial(string name, string partial);
        #endregion
    }
}
