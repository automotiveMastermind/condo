namespace AM.Condo.IO
{
    using System;

    /// <summary>
    /// Defines the properties and methods required to implement a file path manager.
    /// </summary>
    public interface IPathManager : IDisposable
    {
        #region Properties
        /// <summary>
        /// Gets the full path of the temporary path.
        /// </summary>
        string FullPath { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Combines the current path with the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path that should be combined with the current path.
        /// </param>
        /// <returns>
        /// The specified <paramref name="path"/> combined with the current path.
        /// </returns>
        string Combine(string path);

        /// <summary>
        /// Determines whether or not the path controlled by the current path manager exists.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the path controlled by the current path manager exists.
        /// </returns>
        bool Exists();

        /// <summary>
        /// Creates a new path at the specified relative path.
        /// </summary>
        /// <param name="relativePath">
        /// The path that should be created.
        /// </param>
        /// <returns>
        /// The fully-qualified path to the directory that was created.
        /// </returns>
        string Create(string relativePath);

        /// <summary>
        /// Saves a file at the specified <paramref name="relativePath"/> with the specified
        /// <paramref name="contents"/>.
        /// </summary>
        /// <param name="relativePath">
        /// The relative path of the file that should be saved.
        /// </param>
        /// <param name="contents">
        /// The contents of the file.
        /// </param>
        /// <returns>
        /// The fully-qualified path to the file that was saved.
        /// </returns>
        /// <remarks>
        /// This will overwrite any existing file.
        /// </remarks>
        string Save(string relativePath, string contents);
        #endregion
    }
}