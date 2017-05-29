// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitFlowOptions.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    /// <summary>
    /// Represents options used to configure git flow.
    /// </summary>
    public class GitFlowOptions
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the name of the branch used for next release development.
        /// </summary>
        public string NextReleaseBranch { get; set; } = "develop";

        /// <summary>
        /// Gets or sets the name of the branch used for production releases.
        /// </summary>
        public string ProductionReleaseBranch { get; set; } = "master";

        /// <summary>
        /// Gets or sets the prefix of the branches used for feature development.
        /// </summary>
        public string FeatureBranchPrefix { get; set; } = "feature";

        /// <summary>
        /// Gets or sets the prefix of the branches used for bug fix development.
        /// </summary>
        public string BugfixBranchPrefix { get; set; } = "bugfix";

        /// <summary>
        /// Gets or sets the prefix of the branches used for release development.
        /// </summary>
        public string ReleaseBranchPrefix { get; set; } = "release";

        /// <summary>
        /// Gets or sets the prefix of the branches used for hotfix development.
        /// </summary>
        public string HotfixBranchPrefix { get; set; } = "hitfix";

        /// <summary>
        /// Gets or sets the prefix of the branches used for support development.
        /// </summary>
        public string SupportBranchPrefix { get; set; } = "support";

        /// <summary>
        /// Gets or sets the prefix for version tags.
        /// </summary>
        public string VersionTagPrefix { get; set; } = string.Empty;
        #endregion
    }
}
