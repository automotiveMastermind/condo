namespace PulseBridge.Condo.IO
{
    public static partial class GitRepositoryExtensions
    {
        /// <summary>
        /// Tracks changes of all modifications made to all files within the repository.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Add(this IGitRepositoryInitialized repository)
        {
            return repository.Add(".");
        }

        /// <summary>
        /// Creates a default README file with no content at the root of the repository.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Save(this IGitRepositoryInitialized repository)
        {
            return repository.Save("README", string.Empty);
        }

        /// <summary>
        /// Creates a file with the specified <paramref name="relativePath"/> with no content.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <param name="relativePath">
        /// The path of the file relative to the root of the repository that should be created with no content.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Save(this IGitRepositoryInitialized repository, string relativePath)
        {
            return repository.Save(relativePath, string.Empty);
        }

        /// <summary>
        /// Creates a new commit with the specified <paramref name="subject"/>.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <param name="subject">
        /// The subject, or first line of the commit message.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Commit(this IGitRepositoryInitialized repository, string subject)
        {
            return repository.Commit(type: null, scope: null, subject: subject, body: null);
        }

        /// <summary>
        /// Creates a new commit with the specified <paramref name="type"/> and <paramref name="subject"/>.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <param name="type">
        /// The type of the commit.
        /// </param>
        /// <param name="subject">
        /// The subject, or first line of the commit message.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Commit
            (this IGitRepositoryInitialized repository, string type, string subject)
        {
            return repository.Commit(type: type, scope: null, subject: subject, body: null);
        }

        /// <summary>
        /// Creates a new commit with the specified <paramref name="type"/>, <paramref name="scope"/> and
        /// <paramref name="subject"/>.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <param name="type">
        /// The type of the commit.
        /// </param>
        /// <param name="scope">
        /// The scope of the commit.
        /// </param>
        /// <param name="subject">
        /// The subject, or first line of the commit message.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Commit
            (this IGitRepositoryInitialized repository, string type, string scope, string subject)
        {
            return repository.Commit(type: type, scope: scope, subject: subject, body: null);
        }

        /// <summary>
        /// Creates a new branch with the specified <paramref name="name"/>
        /// based on the specified <paramref name="source"/>.
        /// </summary>
        /// <param name="repository">
        /// The current repository instance.
        /// </param>
        /// <param name="name">
        /// The name of the branch to create.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        public static IGitRepositoryInitialized Branch(this IGitRepositoryInitialized repository, string name)
        {
            return repository.Branch(name, source: null);
        }
    }
}