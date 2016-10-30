namespace PulseBridge.Condo.IO
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
        /// Initializes the git repository.
        /// </summary>
        /// <param name="name">
        /// The username that should be used to initialize the git repository.
        /// </param>
        /// <param name="email">
        /// The email address that should be used to initialize the git repository.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Initialize(string name, string email);

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
        #endregion
    }
}