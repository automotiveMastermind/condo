// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetBuildQuality.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Linq;
    using System.Security;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that determines the build quality of the build.
    /// </summary>
    public class GetBuildQuality : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the branch used to determine the pre-release tag.
        /// </summary>
        public string Branch { get; set; } = "<unknown>";

        /// <summary>
        /// Gets or sets a value indicating whether or not the build is a CI build.
        /// </summary>
        public bool CI { get; set; } = false;

        /// <summary>
        /// Gets or sets the prefix used to identify a feature that is in development.
        /// </summary>
        public string FeatureBranchPrefix { get; set; } = "feature";

        /// <summary>
        /// Gets or sets the prefix used to identify a bug fix that is in development.
        /// </summary>
        public string BugfixBranchPrefix { get; set; } = "bugfix";

        /// <summary>
        /// Gets or sets the prefix used to identify a release branch for finalization.
        /// </summary>
        public string ReleaseBranchPrefix { get; set; } = "release";

        /// <summary>
        /// Gets or sets the prefix used to identify a support branch for long-term support.
        /// </summary>
        public string SupportBranchPrefix { get; set; } = "support";

        /// <summary>
        /// Gets or sets the prefix used to identify a hotfix branch for real-time patch development.
        /// </summary>
        public string HotfixBranchPrefix { get; set; } = "hotfix";

        /// <summary>
        /// Gets or sets the name used to identify the integration branch for development.
        /// </summary>
        public string NextReleaseBranch { get; set; } = "develop";

        /// <summary>
        /// Gets or sets the name used to identify the integration branch for production.
        /// </summary>
        public string[] ProductionReleaseBranch { get; set; } = new string[] { "main", "master" };

        /// <summary>
        /// Gets or sets the default build quality, which is used whenever a branch specific build quality is not set
        /// or when a branch purpose cannot be determined.
        /// </summary>
        public string DefaultBuildQuality { get; set; } = "alpha";

        /// <summary>
        /// Gets or sets the build quality to use for the develop branch.
        /// </summary>
        public string NextReleaseBranchBuildQuality { get; set; } = "beta";

        /// <summary>
        /// Gets or sets the build quality to use for the master branch.
        /// </summary>
        public string ProductionReleaseBranchBuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the build quality to use for feature branches.
        /// </summary>
        public string FeatureBranchBuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the build quality to use for bug fix branches.
        /// </summary>
        public string BugfixBranchBuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the build quality to use for release branches.
        /// </summary>
        public string ReleaseBranchBuildQuality { get; set; } = "rc";

        /// <summary>
        /// Gets or sets the build quality to use for support branches.
        /// </summary>
        public string SupportBranchBuildQuality { get; set; } = "servicing";

        /// <summary>
        /// Gets or sets the build quality to use for hotfix branches.
        /// </summary>
        public string HotfixBranchBuildQuality { get; set; } = "hotfix";

        /// <summary>
        /// Gets or sets the pre-release tag.
        /// </summary>
        [Output]
        public string BuildQuality { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to create a release.
        /// </summary>
        [Output]
        public bool CreateRelease { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetAssemblyInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // set the prelrease tag to alpha by default
            this.SetBuildQuality(this.DefaultBuildQuality);

            // only inspect the current branch when running on a build server
            if (!this.CI)
            {
                return true;
            }

            // determine if the branch is the main or master branch
            if (this.ProductionReleaseBranch.Any(b => this.Branch.Equals(b, StringComparison.OrdinalIgnoreCase)))
            {
                // set the build quality to the master branch build quality
                this.SetBuildQuality(this.ProductionReleaseBranchBuildQuality);

                // set the create release flag
                this.CreateRelease = true;

                // move on immediately
                return true;
            }

            // determine if the branch is a develop branch
            if (!string.IsNullOrEmpty(this.NextReleaseBranchBuildQuality)
                && this.Branch.Equals(this.NextReleaseBranch, StringComparison.OrdinalIgnoreCase))
            {
                // set the build quality to the develop branch build quality
                this.SetBuildQuality(this.NextReleaseBranchBuildQuality);

                // move on immediately
                return true;
            }

            // determine if the branch is a feature branch
            if (!string.IsNullOrEmpty(this.FeatureBranchBuildQuality)
                && this.Branch.StartsWith(this.FeatureBranchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // set the build quality to the feature branch build quality
                this.SetBuildQuality(this.FeatureBranchBuildQuality);

                // move on immediately
                return true;
            }

            // determine if the branch is a bugfix branch
            if (!string.IsNullOrEmpty(this.BugfixBranchBuildQuality)
                && this.Branch.StartsWith(this.BugfixBranchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // set the build quality to the bugfix branch build quality
                this.SetBuildQuality(this.BugfixBranchBuildQuality);

                // move on immediately
                return true;
            }

            // determine if the branch is a release branch
            if (!string.IsNullOrEmpty(this.ReleaseBranchBuildQuality)
                && this.Branch.StartsWith(this.ReleaseBranchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // set the build quality to the release branch build quality
                this.SetBuildQuality(this.ReleaseBranchBuildQuality);

                // set the create release flag
                this.CreateRelease = true;

                // move on immediately
                return true;
            }

            // determine if the branch is a support branch
            if (!string.IsNullOrEmpty(this.SupportBranchBuildQuality)
                && this.Branch.StartsWith(this.SupportBranchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // set the build quality to the support branch build quality
                this.SetBuildQuality(this.SupportBranchBuildQuality);

                // set the create release flag
                this.CreateRelease = true;

                // move on immediately
                return true;
            }

            // determine if the branch is a hotfix branch
            if (!string.IsNullOrEmpty(this.HotfixBranchBuildQuality)
                && this.Branch.StartsWith(this.HotfixBranchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // set the build quality to the hotfix branch build quality
                this.SetBuildQuality(this.HotfixBranchBuildQuality);

                // set the create release flag
                this.CreateRelease = true;

                // move on immediately
                return true;
            }

            // assume there is nothing to do
            return true;
        }

        private void SetBuildQuality(string quality)
        {
            this.BuildQuality = quality;

            try
            {
                Environment.SetEnvironmentVariable(nameof(this.BuildQuality), this.BuildQuality);
            }
            catch (SecurityException securityEx)
            {
                this.Log.LogWarningFromException(securityEx);
            }
        }
        #endregion
    }
}
