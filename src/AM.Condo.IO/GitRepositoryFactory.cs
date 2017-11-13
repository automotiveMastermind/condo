// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitRepositoryFactory.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using AM.Condo.Diagnostics;

    /// <summary>
    /// Represents a default implementation of a git repository factory.
    /// </summary>
    public class GitRepositoryFactory : IGitRepositoryFactory
    {
        #region Methods
        /// <inheritdoc />
        public IGitRepositoryInitialized Clone(IPathManager path, string uri, string authorization, ILogger logger)
        {
            // initialize the repository on the given path
            var repository = new GitRepository(path, logger);

            // determine if the authorization is specified
            if (!string.IsNullOrEmpty(authorization))
            {
                // set the authorization
                repository.GlobalConfig($"http.{uri}.extraheader", authorization);
            }

            // clone the repository
            return repository.Clone(uri);
        }

        /// <inheritdoc />
        public IGitRepositoryBare Bare(IPathManager path, ILogger logger)
        {
            // create a bare repository
            return new GitRepository(path, logger).Bare();
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized Load(IPathManager path, ILogger logger)
        {
            return new GitRepository(path, logger) as IGitRepositoryInitialized;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized Initialize(IPathManager path, ILogger logger)
        {
            return new GitRepository(path, logger).Initialize();
        }
        #endregion
    }
}
