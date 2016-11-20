namespace PulseBridge.Condo.IO
{
    public static partial class GitRepositoryExtensions
    {
        private static readonly IGitLogOptions Angular = new AngularGitLogOptions();

        private static readonly IGitLogParser Parser = new GitLogParser();

        public static IGitLog Log(this IGitRepositoryInitialized repository)
        {
            return repository.Log(from: null, to: null, options: Angular, parser: Parser);
        }
    }
}