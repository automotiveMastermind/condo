// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGitFlow.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    /// <summary>
    /// Defines the properties and methods required to implement a repository provider for git flow.
    /// </summary>
    public interface IGitFlow
    {
        #region Methods
        /// <summary>
        /// Initializes git flow with the default options.
        /// </summary>
        /// <param name="options">
        /// The options used to configure the git flow. If these options are <see langword="null"/>, then the options
        /// used to initialize the repository will be used.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized GitFlow(GitFlowOptions options);

        /// <summary>
        /// Starts a new flow of the specified <paramref name="type"/> with the specified <paramref name="name"/> using
        /// the specified <paramref name="source"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the flow to start.
        /// </param>
        /// <param name="name">
        /// The name of the flow.
        /// </param>
        /// <param name="source">
        /// The source branch used as the base for the flow.
        /// </param>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized StartFlow(string type, string name, string source);

        /// <summary>
        /// Finishes a current flow, such as a feature, bugfix, hotfix, etc.
        /// </summary>
        /// <returns>
        /// The current repository instance.
        /// </returns>
        IGitRepositoryInitialized FinishFlow();
        #endregion
    }
}
