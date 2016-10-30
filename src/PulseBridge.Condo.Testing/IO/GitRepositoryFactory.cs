namespace PulseBridge.Condo.IO
{
    /// <summary>
    /// Represents a default implementation of a git repository factory.
    /// </summary>
    public class GitRepositoryFactory : IGitRepositoryFactory
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
        public IGitRepositoryInitialized Clone(string uri, string path)
        {
            // clone the git repository
            return new GitRepository(path).Clone(uri);
        }

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
        public IGitRepositoryInitialized Clone(string uri)
        {
            // clone the git repository
            return new GitRepository().Clone(uri);
        }

        /// <summary>
        /// Initializes a new bare repository.
        /// </summary>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        public IGitRepositoryBare Bare()
        {
            // create a bare repository
            return new GitRepository().Bare();
        }

        /// <summary>
        /// Initializes a new git repository.
        /// </summary>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        public IGitRepositoryInitialized Initialize()
        {
            return new GitRepository().Initialize();
        }

        /// <summary>
        /// Initializes a new git repository at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path in which the initialized repository should be created.
        /// </param>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        public IGitRepositoryInitialized Initialize(string path)
        {
            return new GitRepository(path).Initialize();
        }
        #endregion
    }
}