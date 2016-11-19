namespace PulseBridge.Condo.IO
{
    public static class GitRepositoryFactoryExtensions
    {
        /// <summary>
        /// Initializes a new git repository.
        /// </summary>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        public static IGitRepositoryInitialized Initialize(this IGitRepositoryFactory factory)
        {
            return factory.Initialize(new TemporaryPath());
        }

        /// <summary>
        /// Creates a new bare git repository.
        /// </summary>
        /// <returns>
        /// A newly created bare git repository.
        /// </returns>
        public static IGitRepositoryBare Bare(this IGitRepositoryFactory factory)
        {
            return factory.Bare(new TemporaryPath());
        }

        /// <summary>
        /// Creates a new git repository instance after cloning the repository from the specified
        /// <paramref name="uri"/> into the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="factory">
        /// The current factory instance.
        /// </param>
        /// <param name="uri">
        /// The URI of the git repository that should be cloned.
        /// </param>
        /// <returns>
        /// A new git repository instance that is tracking a cloned repository.
        /// </returns>
        public static IGitRepositoryInitialized Clone(this IGitRepositoryFactory factory, string uri)
        {
            // clone the git repository
            return factory.Clone(new TemporaryPath(), uri);
        }

        /// <summary>
        /// Loads the repository at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="factory">
        /// The current factory instance.
        /// </param>
        /// <param name="path">
        /// The path that should be an existing git repository.
        /// </param>
        /// <returns>
        /// The git repository at the specified <paramref name="path"/>.
        /// </returns>
        public static IGitRepositoryInitialized Load(this IGitRepositoryFactory factory, string path)
        {
            // clone the git repository
            return factory.Load(new PathManager(path));
        }

        /// <summary>
        /// Creates a new git repository instance after cloning the repository from the specified
        /// <paramref name="uri"/> into the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="factory">
        /// The current factory instance.
        /// </param>
        /// <param name="uri">
        /// The URI of the git repository that should be cloned.
        /// </param>
        /// <param name="path">
        /// The path in which the cloned repository should be created.
        /// </param>
        /// <returns>
        /// A new git repository instance that is tracking a cloned repository.
        /// </returns>
        public static IGitRepositoryInitialized Clone(this IGitRepositoryFactory factory, string uri, string path)
        {
            // clone the git repository
            return factory.Clone(new PathManager(path), uri);
        }
    }
}