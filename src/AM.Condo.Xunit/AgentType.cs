// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentType.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;

    /// <summary>
    /// Represents an enumeration of the available agents (build) types allowed for a test.
    /// </summary>
    [Flags]
    public enum AgentType
    {
        /// <summary>
        /// The agent type indicates that a test can be executed for continuous integration builds.
        /// </summary>
        CI = 1,

        /// <summary>
        /// The agent type indicates that a test can be executed for local builds.
        /// </summary>
        Local = 2,

        /// <summary>
        /// The agent type indicates that a test can be executed for any build.
        /// </summary>
        Any = ~0
    }
}
