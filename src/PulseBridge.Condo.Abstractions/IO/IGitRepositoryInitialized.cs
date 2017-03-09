namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

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

        /// <summary>
        /// Gets the URI of the origin remote.
        /// </summary>
        string OriginUri { get; }

        /// <summary>
        /// Gets the list of tags associated with the repository.
        /// </summary>
        IEnumerable<string> Tags { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Attempts to pick out and massage the specified reference.
        /// </summary>
        /// <param name="reference">
        /// The reference to pick out and massage.
        /// </param>
        /// <returns>
        /// The SHA1 of the specified <paramref name="reference"/>.
        /// </returns>
        string RevParse(string reference);

        /// <summary>
        /// Saves a file at the specified <paramref name="relativePath"/> with the specified
        /// <paramref name="contents"/>.
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
        /// Creates a new commit with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// The message used to describe the commit.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Commit(string message);

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
        IGitRepositoryInitialized Branch(string name, string source);

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
        /// Restores all available submodules within the current repository.
        /// </summary>
        /// <param name="recursive">
        /// A value indicating whether or not to recursively restore submodules.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized RestoreSubmodules(bool recursive);

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

        /// <summary>
        /// Pushes all staged changes to the specified <paramref name="remote"/> and optionally includes tags.
        /// </summary>
        /// <param name="remote">
        /// The remote to which to push the staged changes.
        /// </param>
        /// <param name="tags">
        /// A value indicating whether or not to include tags.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Push(string remote, bool tags);

        /// <summary>
        /// Pulls the current branch or all branches from the remote repository.
        /// </summary>
        /// <param name="all">
        /// A value indicating whether or not to pull all branches.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized Pull(bool all);

        /// <summary>
        /// Sets the URL of the remote with the specified <paramref name="name"/> to the specified
        /// <paramref name="uri"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the remote to set.
        /// </param>
        /// <param name="uri">
        /// The URI of the remote.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized SetRemoteUrl(string name, string uri);

        /// <summary>
        /// Adds a remote with the specified <paramref name="name"/> and <paramref name="uri"/> to the repository.
        /// </summary>
        /// <param name="name">
        /// The name of the remote to add.
        /// </param>
        /// <param name="uri">
        /// The URI of the remote to add.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized AddRemote(string name, string uri);

        /// <summary>
        /// Gets the git log using the specified <paramref name="options"/>
        /// </summary>
        /// <param name="from">
        /// The item specification from which the git log should start.
        /// </param>
        /// <param name="to">
        /// The item specification to which the git log should end.
        /// </param>
        /// <param name="options">
        /// The options used to parse the git log.
        /// </param>
        /// <param name="parser">
        /// The parser used to get the log.
        /// </param>
        /// <returns>
        /// The git log for the specified <paramref name="parser"/>.
        /// </returns>
        GitLog Log(string from, string to, IGitLogOptions options, IGitLogParser parser);
        #endregion
    }
}
