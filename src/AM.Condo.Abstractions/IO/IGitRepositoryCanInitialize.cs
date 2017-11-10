// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGitRepositoryCanInitialize.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;

    /// <summary>
    /// Defines the properties and methods required to implement a git repository that can be initialized.
    /// </summary>
    public interface IGitRepositoryCanInitialize : IDisposable
    {
        #region Methods
        /// <summary>
        /// Initializes the git repository.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Initialize();

        /// <summary>
        /// Initializes the git repository as a bare repository.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryBare Bare();

        /// <summary>
        /// Clones the repository from the specified <paramref name="uri"/> into the root of the current repository
        /// path.
        /// </summary>
        /// <param name="uri">
        /// The URI of the repository that should be cloned.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Clone(string uri);

        /// <summary>
        /// Sets the configuration <paramref name="value"/> for the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The key for the specified <paramref name="value"/>.
        /// </param>
        /// <param name="value">
        /// The value to set for the specified <paramref name="key"/>.
        /// </param>
        void GlobalConfig(string key, string value);
        #endregion
    }
}
