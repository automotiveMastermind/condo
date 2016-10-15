namespace PulseBridge.Condo.IO
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
        /// Combines the current temporary path with the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path that should be combined with the current temporary path.
        /// </param>
        /// <returns>
        /// The specified <paramref name="path"/> combined with the current temporary path.
        /// </returns>
        string Combine(string path);

        /// <summary>
        /// Determines whether or not the path controlled by the current path manager exists.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the path controlled by the current path manager exists.
        /// </returns>
        bool Exists();
        #endregion
    }
}