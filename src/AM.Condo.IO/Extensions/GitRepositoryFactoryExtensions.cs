namespace AM.Condo.IO
{
    using AM.Condo.Diagnostics;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="IGitRepositoryFactory"/> interface.
    /// </summary>
    public static class GitRepositoryFactoryExtensions
    {
        #region Private Fields
        private static readonly ILogger NoOpLogger = new NoOpLogger();
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new git repository.
        /// </summary>
        /// <param name="factory">
        /// The factory used to initialize a new repository.
        /// </param>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        public static IGitRepositoryInitialized Initialize(this IGitRepositoryFactory factory)
        {
            return factory.Initialize(NoOpLogger);
        }

        /// <summary>
        /// Initializes a new git repository.
        /// </summary>
        /// <param name="factory">
        /// The factory used to initialize a new repository.
        /// </param>
        /// <param name="logger">
        /// The logger used by the repository.
        /// </param>
        /// <returns>
        /// A newly initialized git repository.
        /// </returns>
        public static IGitRepositoryInitialized Initialize(this IGitRepositoryFactory factory, ILogger logger)
        {
            return factory.Initialize(new TemporaryPath(), logger);
        }

        /// <summary>
        /// Creates a new bare git repository.
        /// </summary>
        /// <param name="factory">
        /// The factory used to initialize a new repository.
        /// </param>
        /// <returns>
        /// A newly created bare git repository.
        /// </returns>
        public static IGitRepositoryBare Bare(this IGitRepositoryFactory factory)
        {
            return factory.Bare(NoOpLogger);
        }

        /// <summary>
        /// Creates a new bare git repository.
        /// </summary>
        /// <param name="factory">
        /// The factory used to initialize a new repository.
        /// </param>
        /// <param name="logger">
        /// The logger used by the repository.
        /// </param>
        /// <returns>
        /// A newly created bare git repository.
        /// </returns>
        public static IGitRepositoryBare Bare(this IGitRepositoryFactory factory, ILogger logger)
        {
            return factory.Bare(new TemporaryPath(), logger);
        }

        /// <summary>
        /// Creates a new git repository instance after cloning the repository from the specified
        /// <paramref name="uri"/>.
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
            return factory.Clone(uri, NoOpLogger);
        }

        /// <summary>
        /// Creates a new git repository instance after cloning the repository from the specified
        /// <paramref name="uri"/>.
        /// </summary>
        /// <param name="factory">
        /// The current factory instance.
        /// </param>
        /// <param name="uri">
        /// The URI of the git repository that should be cloned.
        /// </param>
        /// <param name="logger">
        /// The logger used by the repository.
        /// </param>
        /// <returns>
        /// A new git repository instance that is tracking a cloned repository.
        /// </returns>
        public static IGitRepositoryInitialized Clone(this IGitRepositoryFactory factory, string uri, ILogger logger)
        {
            // clone the git repository
            return factory.Clone(new TemporaryPath(), uri, logger);
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
            return factory.Load(path, NoOpLogger);
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
        /// <param name="logger">
        /// The logger used by the repository.
        /// </param>
        /// <returns>
        /// The git repository at the specified <paramref name="path"/>.
        /// </returns>
        public static IGitRepositoryInitialized Load(this IGitRepositoryFactory factory, string path, ILogger logger)
        {
            return factory.Load(new PathManager(path), logger);
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
            return factory.Clone(path, uri, NoOpLogger);
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
        /// <param name="logger">
        /// The logger used by the repository.
        /// </param>
        /// <returns>
        /// A new git repository instance that is tracking a cloned repository.
        /// </returns>
        public static IGitRepositoryInitialized Clone
            (this IGitRepositoryFactory factory, string uri, string path, ILogger logger)
        {
            // clone the git repository
            return factory.Clone(new PathManager(path), uri, logger);
        }
        #endregion
    }
}
