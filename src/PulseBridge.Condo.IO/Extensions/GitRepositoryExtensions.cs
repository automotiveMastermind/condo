namespace PulseBridge.Condo.IO
{
    /// <summary>
    /// Represents a set of extension methods for the <see cref="IGitRepository"/> interface.
    /// </summary>
    public static class GitRepositoryExtensions
    {
        private static readonly IGitLogOptions Angular = new AngularGitLogOptions();

        private static readonly IGitLogParser Parser = new GitLogParser();

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
            return repository.Log(from: null, to: null, options: Angular, parser: Parser);
        }
    }
}