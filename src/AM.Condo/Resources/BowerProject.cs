// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BowerProject.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Resources
{
    /// <summary>
    /// Represents a bower project (bower.json).
    /// </summary>
    public class BowerProject
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
        #endregion
    }
}
