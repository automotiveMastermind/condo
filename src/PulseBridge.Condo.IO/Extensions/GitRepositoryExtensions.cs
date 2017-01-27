namespace PulseBridge.Condo.IO
{
    /// <summary>
    /// Represents a set of extension methods for the <see cref="IGitRepository"/> interface.
    /// </summary>
    public static class GitRepositoryExtensions
    {
        #region Private Fields
        private static readonly IGitLogOptions Options = new GitLogOptions();

        private static readonly IGitLogParser Parser = new GitLogParser();
        #endregion

        #region Methods
        /// <summary>
        /// Pushes any staged changes to the "origin" remote and includes tags.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Push(this IGitRepositoryInitialized repository)
        {
            return repository.Push(remote: "origin", tags: true);
        }

        /// <summary>
        /// Pushes any staged changes to the "origin" remote and optionally includes tags.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <param name="tags">
        /// A value indicating whether or not to include tags.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Push(this IGitRepositoryInitialized repository, bool tags)
        {
            return repository.Push(remote: "origin", tags: tags);
        }

        /// <summary>
        /// Gets the log of commmits from a repository using the default log options, which are based on the AngularJS
        /// conventional changelog presets.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <returns>
        /// The log of commits from the repository using the default log options, which are based on the AngularJS
        /// conventional changelog presets.
        /// </returns>
        public static GitLog Log(this IGitRepositoryInitialized repository)
        {
            return repository.Log(from: null, to: null, options: Options, parser: Parser);
        }
        #endregion
    }
}
