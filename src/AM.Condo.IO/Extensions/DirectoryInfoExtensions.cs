// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystem.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="DirectoryInfo"/> class.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        #region Methods
        /// <summary>
        /// Copies all of the files located in the specified <paramref name="source"/> to the specified
        /// <paramref name="destination"/>.
        /// </summary>
        /// <param name="source">
        /// The source directory containing the files to copy.
        /// </param>
        /// <param name="destination">
        /// The destination directory to which the files should be copied.
        /// </param>
        /// <param name="overwrite">
        /// A value indicating whether or not to overwrite existing files.
        /// </param>
        /// <param name="recursive">
        /// A value indicating whether or not to copy files recursively from sub-directories within the source.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the specified <paramref name="source"/> or <paramref name="destination"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified <paramref name="destination"/> is empty.
        /// </exception>
        public static void CopyTo(this DirectoryInfo source, string destination, bool overwrite, bool recursive)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination.Equals(string.Empty))
            {
                throw new ArgumentException($"The {nameof(destination)} cannot be empty.", nameof(destination));
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in source.EnumerateFiles())
            {
                file.CopyTo(Path.Combine(destination, file.Name));
            }

            if (!recursive)
            {
                return;
            }

            foreach (var directory in source.EnumerateDirectories())
            {
                directory.CopyTo(Path.Combine(destination, directory.Name), overwrite, recursive);
            }
        }
        #endregion
    }
}
