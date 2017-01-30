namespace PulseBridge.Condo.IO
{
    using System;

    using PulseBridge.Condo.Diagnostics;

    /// <summary>
    /// Defines the properties and methods required to implement a git repository that is bare.
    /// </summary>
    public interface IGitRepositoryBare : IDisposable
    {
        #region Properties
        /// <summary>
        /// Gets the current branch.
        /// </summary>
        string CurrentBranch { get; }

        /// <summary>
        /// Gets the latest commit on the current branch.
        /// </summary>
        string LatestCommit { get; }

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
        /// Executes the specified <paramref name="command"/> using the git command line tool.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <returns>
        /// The output from the process.
        /// </returns>
        IProcessOutput Execute(string command);
        #endregion
    }
}