// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeProject.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Resources
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a node project (package.json).
    /// </summary>
    public class NodeProject
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of the project.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the license for the project.
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Gets the scripts for the project.
        /// </summary>
        public Dictionary<string, string> Scripts { get; set; }
        #endregion
    }
}
