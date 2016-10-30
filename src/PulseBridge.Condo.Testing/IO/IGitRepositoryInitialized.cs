namespace PulseBridge.Condo.IO
{
    /// <summary>
    /// Defines the properties and methods required to implement a git repository that has already been initialized.
    /// </summary>
    public interface IGitRepositoryInitialized : IGitRepositoryBare
    {
        #region Properties
        /// <summary>
        /// Gets or sets the current username associated with the repository configuration.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Gets or sets the current email associated with the repository configuration.
        /// </summary>
        string Email { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a default README file with no content at the root of the repository.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Save();

        /// <summary>
        /// Creates a file with the specified <paramref name="relativePath"/> with no content.
        /// </summary>
        /// <param name="relativePath">
        /// The path of the file relative to the root of the repository that should be created with no content.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Save(string relativePath);

        /// <summary>
        /// Saves a file at the specified <paramref name="relativePath"/> with the specified <param name="contents"/>.
        /// </summary>
        /// <param name="relativePath">
        /// The relative path of the file that should be saved.
        /// </param>
        /// <param name="contents">
        /// The contents of the file.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        /// <remarks>
        /// This will overwrite any existing file.
        /// </remarks>
        IGitRepositoryInitialized Save(string relativePath, string contents);

        /// <summary>
        /// Tracks changes of all modifications made to all files within the repository.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Add();

        /// <summary>
        /// Tracks changes to the files included by the specified <paramref name="spec"/> within the current commit
        /// context.
        /// </summary>
        /// <param name="spec">
        /// The file specification used to track changes to one or more files.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Add(string spec);

        /// <summary>
        /// Creates a new commit with the specified <paramref name="subject"/>.
        /// </summary>
        /// <param name="subject">
        /// The subject, or first line of the commit message.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Commit(string subject);

        /// <summary>
        /// Creates a new commit with the specified <paramref name="type"/> and <paramref name="subject"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the commit.
        /// </param>
        /// <param name="subject">
        /// The subject, or first line of the commit message.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Commit(string type, string subject);

        /// <summary>
        /// Creates a new commit with the specified <paramref name="type"/>, <paramref name="scope"/> and
        /// <paramref name="subject"/>.
        /// </summary>
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
        IGitRepositoryInitialized Commit(string type, string scope, string subject);

        /// <summary>
        /// Creates a new commit with the specified <paramref name="type"/>, <paramref name="scope"/> and
        /// <paramref name="subject"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the commit.
        /// </param>
        /// <param name="scope">
        /// The scope of the commit.
        /// </param>
        /// <param name="subject">
        /// The subject, or first line of the commit message.
        /// </param>
        /// <param name="body">
        /// The body of the commit message.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Commit(string type, string scope, string subject, string body);

        /// <summary>
        /// Creates a new branch with the specified <paramref name="name"/>
        /// based on the specified <paramref name="source"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the branch to create.
        /// </param>
        /// <param name="source">
        /// The source branch that should be used as the base for the newly created branch.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Branch(string name, string source = null);

        /// <summary>
        /// Checks out the branch or tag with the specified <paramref name="name"/>.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Checkout(string name);

        /// <summary>
        /// Creates a tag with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the tag to create.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Tag(string name);

        /// <summary>
        /// Initializes condo within the current repository using the specified <paramref name="root"/> path to locate
        /// the source for condo and configuring the build system.
        /// </summary>
        /// <param name="root">
        /// The root path containing the source code for condo.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Condo(string root);
        #endregion
    }
}