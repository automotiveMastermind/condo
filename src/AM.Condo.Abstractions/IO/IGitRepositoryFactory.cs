// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGitRepositoryFactory.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using AM.Condo.Diagnostics;

    /// <summary>
    /// Defines the properties and methods required to implement a factory for creating git repositories.
    /// </summary>
    public interface IGitRepositoryFactory
    {
        #region Methods
        /// <summary>
        /// Creates a new git repository instance after cloning the repository from the specified
        /// <paramref name="uri"/> into the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path manager in which the cloned repository should be created.
        /// </param>
        /// <param name="uri">
        /// The URI of the git repository that should be cloned.
        /// </param>
        /// <param name="authorization">
        /// The authorization header to use for access the repository.
        /// </param>
        /// <param name="logger">
        /// The logger to use with the repository.
        /// </param>
        /// <returns>
        /// A new git repository instance that is tracking a cloned repository.
        /// </returns>
        IGitRepositoryInitialized Clone(IPathManager path, string uri, string authorization, ILogger logger);

        /// <summary>
        /// Initializes a new bare repository.
        /// </summary>
        /// <param name="path">
        /// The path manager in which to create the bare repository.
        /// </param>
        /// <param name="logger">
        /// The logger to use with the repository.
        /// </param>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        IGitRepositoryBare Bare(IPathManager path, ILogger logger);

        /// <summary>
        /// Initializes a new git repository.
        /// </summary>
        /// <param name="path">
        /// The path manager in which to create the bare repository.
        /// </param>
        /// <param name="logger">
        /// The logger to use with the repository.
        /// </param>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        IGitRepositoryInitialized Initialize(IPathManager path, ILogger logger);

        /// <summary>
        /// Loads an existing git repository.
        /// </summary>
        /// <param name="path">
        /// The path manager in which to create the bare repository.
        /// </param>
        /// <param name="logger">
        /// The logger to use with the repository.
        /// </param>
        /// <returns>
        /// An existing git repository.
        /// </returns>
        IGitRepositoryInitialized Load(IPathManager path, ILogger logger);
        #endregion
    }
}
