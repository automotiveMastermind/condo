namespace PulseBridge.Condo.IO
{
    using System;

    using PulseBridge.Condo.Diagnostics;

    /// <summary>
    /// Defines the properties and methods required to implement a git repository.
    /// </summary>
    public interface IGitRepository : IDisposable
    {
        #region Properties
        /// <summary>
        /// Gets the current branch.
        /// </summary>
        string CurrentBranch { get; }

        /// <summary>
        /// Gets the version of the git client in use on the system.
        /// </summary>
        string ClientVersion { get; }

        /// <summary>
        /// Gets the current fully-qualified path to the repository.
        /// </summary>
        string RepositoryPath { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the git repository.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepository Initialize();

        /// <summary>
        /// Initializes the git repository.
        /// </summary>
        /// <param name="name">
        /// The username that should be used to initialize the git repository.
        /// </param>
        /// <param name="email">
        /// The email address that should be used to initialize the git repository.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepository Initialize(string name, string email);

        /// <summary>
        /// Creates a default README file with no content at the root of the repository.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepository Save();

        /// <summary>
        /// Creates a file with the specified <paramref name="relativePath"/> with no content.
        /// </summary>
        /// <param name="relativePath">
        /// The path of the file relative to the root of the repository that should be created with no content.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepository Save(string relativePath);

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
        IGitRepository Save(string relativePath, string contents);

        /// <summary>
        /// Tracks changes of all modifications made to all files within the repository.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepository Add();

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
        IGitRepository Add(string spec);

        /// <summary>
        /// Creates a new commit with the specified <paramref name="subject"/>.
        /// </summary>
        /// <param name="subject">
        /// The subject, or first line of the commit message.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepository Commit(string subject);

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
        IGitRepository Commit(string type, string subject);

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
        IGitRepository Commit(string type, string scope, string subject);

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
        IGitRepository Commit(string type, string scope, string subject, string body);


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
        IGitRepository Branch(string name, string source = null);

        /// <summary>
        /// Checks out the branch or tag with the specified <paramref name="name"/>.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepository Checkout(string name);

        /// <summary>
        /// Creates a tag with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the tag to create.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepository Tag(string name);

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
        IGitRepository Condo(string root);

        /// <summary>
        /// Executes the specified <paramref name="command"/> using the git command line tool.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="workingPath">
        /// The path in which to execute the command.
        /// </param>
        /// <returns>
        /// The output from the process.
        /// </returns>
        IProcessOutput Execute(string command);
        #endregion
    }
}