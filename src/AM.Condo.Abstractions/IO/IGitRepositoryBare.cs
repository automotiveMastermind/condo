// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGitRepositoryBare.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;

    using AM.Condo.Diagnostics;

    /// <summary>
    /// Defines the properties and methods required to implement a git repository that is bare.
    /// </summary>
    public interface IGitRepositoryBare : IDisposable
    {
        #region Properties and Indexers
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
        Version ClientVersion { get; }

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
        /// <param name="throwOnError">
        /// A value indicating whether or not to throw an exception in the event that the command does not succeed.
        /// </param>
        /// <returns>
        /// The output from the process.
        /// </returns>
        IProcessOutput Execute(string command, bool throwOnError = false);
        #endregion
    }
}
