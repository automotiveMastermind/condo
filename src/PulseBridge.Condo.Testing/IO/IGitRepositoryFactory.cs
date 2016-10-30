namespace PulseBridge.Condo.IO
{
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
        /// <param name="uri">
        /// The URI of the git repository that should be cloned.
        /// </param>
        /// <param name="path">
        /// The path in which the cloned repository should be created.
        /// </param>
        /// <returns>
        /// A new git repository instance that is tracking a cloned repository.
        /// </returns>
        IGitRepositoryInitialized Clone(string uri, string path);

        /// <summary>
        /// Creates a new git repository instance after cloning the repository from the specified
        /// <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">
        /// The URI of the git repository that should be cloned.
        /// </param>
        /// <returns>
        /// A new git repository instance that is tracking a cloned repository.
        /// </returns>
        IGitRepositoryInitialized Clone(string uri);

        /// <summary>
        /// Initializes a new bare repository.
        /// </summary>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        IGitRepositoryBare Bare();

        /// <summary>
        /// Initializes a new git repository.
        /// </summary>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        IGitRepositoryInitialized Initialize();

        /// <summary>
        /// Initializes a new git repository at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path in which the initialized repository should be created.
        /// </param>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        IGitRepositoryInitialized Initialize(string path);
        #endregion
    }
}