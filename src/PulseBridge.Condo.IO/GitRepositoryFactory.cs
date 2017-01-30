namespace PulseBridge.Condo.IO
{
    /// <summary>
    /// Represents a default implementation of a git repository factory.
    /// </summary>
    public class GitRepositoryFactory : IGitRepositoryFactory
    {
        #region Methods
        /// <inheritdoc />
        public IGitRepositoryInitialized Clone(IPathManager path, string uri)
        {
            // clone the git repository
            return new GitRepository(path).Clone(uri);
        }

        /// <inheritdoc />
        public IGitRepositoryBare Bare(IPathManager path)
        {
            // create a bare repository
            return new GitRepository(path).Bare();
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized Load(IPathManager path)
        {
            return new GitRepository(path) as IGitRepositoryInitialized;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized Initialize(IPathManager path)
        {
            return new GitRepository(path).Initialize();
        }
        #endregion
    }
}