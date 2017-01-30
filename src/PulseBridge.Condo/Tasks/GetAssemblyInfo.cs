namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.Globalization;

    using static System.FormattableString;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that accumulates assembly information.
    /// </summary>
    public class GetAssemblyInfo : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the semantic version of the product.
        /// </summary>
        [Required]
        public string CurrentRelease { get; set; }

        /// <summary>
        /// Gets or sets the date and time (UTC) that the project was first started.
        /// </summary>
        [Required]
        public string StartDateUtc { get; set; }

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
        public string DevelopBranch { get; set; } = "develop";

        /// <summary>
        /// Gets or sets the name used to identify the integration branch for production.
        /// </summary>
        public string MasterBranch { get; set; } = "master";

        /// <summary>
        /// Gets or sets the default build quality, which is used whenever a branch specific build quality is not set
        /// or when a branch purpose cannot be determined.
        /// </summary>
        public string DefaultBuildQuality { get; set; } = "alpha";

        /// <summary>
        /// Gets or sets the build quality to use for the develop branch.
        /// </summary>
        public string DevelopBranchBuildQuality { get; set; } = "beta";

        /// <summary>
        /// Gets or sets the build quality to use for the master branch.
        /// </summary>
        public string MasterBranchBuildQuality { get; set; }

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
        /// Gets the pre-release tag (semantic version suffix) used by dotnet projects.
        /// </summary>
        [Output]
        public string PreReleaseTag { get; private set; }

        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        [Output]
        public string AssemblyVersion { get; private set; }

        /// <summary>
        /// Gets or sets the file version.
        /// </summary>
        [Output]
        public string FileVersion { get; private set; }

        /// <summary>
        /// Gets or sets the pre-release tag.
        /// </summary>
        [Output]
        public string BuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the informational version.
        /// </summary>
        [Output]
        public string InformationalVersion { get; private set; }

        /// <summary>
        /// Gets or sets the build ID used to determine the version.
        /// </summary>
        [Output]
        public string BuildId { get; set; }

        /// <summary>
        /// Gets or sets the commit ID used to determine the version.
        /// </summary>
        [Output]
        public string CommitId { get; set; }

        /// <summary>
        /// Sets the date and time used to determine the version.
        /// </summary>
        [Output]
        public string BuildDateUtc { get; set; }
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
            // determine if the date and time is set
            if (string.IsNullOrEmpty(this.BuildDateUtc))
            {
                // set the date and time to the current time
                this.BuildDateUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(this.StartDateUtc))
            {
                // log an error
                Log.LogError(Invariant($"{nameof(StartDateUtc)} property must be set."));

                // move on immediately
                return false;
            }

            // define a variable to retain the date
            DateTime now;

            // attempt to parse the date
            if (!DateTime.TryParse(this.BuildDateUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out now))
            {
                // log an error
                Log.LogError(Invariant($"{nameof(BuildDateUtc)} property could not be parsed. Please verify that the date is valid."));

                // could not parse; move on immediately
                return false;
            }

            // ensure that the date is always universal time
            now = now.ToUniversalTime();

            // define a variable to retain the start date
            DateTime start;

            // attempt to parse the start date
            if (!DateTime.TryParse(this.StartDateUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out start))
            {
                // log an error
                Log.LogError(Invariant($"{nameof(StartDateUtc)} property could not be parsed. Please verify that the date is valid."));

                // could not parse; move on immediately
                return false;
            }

            // ensure that the date is always universal time
            start = start.ToUniversalTime();

            // determine if the start date is after the current date
            if (start > now)
            {
                // log an error
                Log.LogError(Invariant($"The {nameof(StartDateUtc)} cannot be after the {nameof(BuildDateUtc)}."));

                // start date is after now; move on immediately
                return false;
            }

            // define a variable to retain the version
            Version version;

            // attempt to parse the version
            if (!Version.TryParse(this.CurrentRelease, out version))
            {
                // emit an error if the version is not parsable
                this.Log.LogError(Invariant($"Could not parse {nameof(this.CurrentRelease)} {this.CurrentRelease} -- it is not in the expected format."));

                // move on immediately
                return false;
            }

            // set the assembly version to the semantic version
            this.AssemblyVersion = version.ToString();

            // create a commit number from the current seconds
            var commit = now.ToString("HHmm", CultureInfo.InvariantCulture);

            // determine if the commit id is not already set
            if (string.IsNullOrEmpty(this.CommitId))
            {
                // set the commit id
                this.CommitId = commit;
            }

            // determine if the build id is not already set
            if (string.IsNullOrEmpty(this.BuildId))
            {
                // create a build number based on the number of years that have passed (and the current day of the year)
                var build = (now.Year - start.Year).ToString("D2", CultureInfo.InvariantCulture)
                    + now.DayOfYear.ToString("D3", CultureInfo.InvariantCulture);

                // set the build id
                this.BuildId = build;
            }

            // set the file version
            this.FileVersion = Invariant($"{version.Major}.{version.Minor}.{this.BuildId}.{commit}");

            // determine if the prerelease tag is not already set
            if (string.IsNullOrEmpty(this.BuildQuality))
            {
                // set the prelrease tag to alpha by default
                this.BuildQuality = this.DefaultBuildQuality;

                // only inspect the current branch when running on a build server
                if (this.CI)
                {
                    // determine if the branch is the master branch
                    if (this.Branch.Equals(this.MasterBranch, StringComparison.OrdinalIgnoreCase))
                    {
                        // set the build quality to the master branch build quality
                        this.BuildQuality = this.MasterBranchBuildQuality;
                    }

                    // determine if the branch is a develop branch
                    else if (!string.IsNullOrEmpty(this.DevelopBranchBuildQuality)
                        && this.Branch.Equals(this.DevelopBranch, StringComparison.OrdinalIgnoreCase))
                    {
                        // set the build quality to the develop branch build quality
                        this.BuildQuality = this.DevelopBranchBuildQuality;
                    }

                    // determine if the branch is a feature branch
                    else if (!string.IsNullOrEmpty(this.FeatureBranchBuildQuality)
                        && this.Branch.StartsWith(this.FeatureBranchPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        // set the build quality to the feature branch build quality
                        this.BuildQuality = this.FeatureBranchBuildQuality;
                    }

                    // determine if the branch is a bugfix branch
                    else if (!string.IsNullOrEmpty(this.BugfixBranchBuildQuality)
                        && this.Branch.StartsWith(this.BugfixBranchPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        // set the build quality to the bugfix branch build quality
                        this.BuildQuality = this.BugfixBranchBuildQuality;
                    }

                    // determine if the branch is a release branch
                    else if (!string.IsNullOrEmpty(this.ReleaseBranchBuildQuality)
                        && this.Branch.StartsWith(this.ReleaseBranchPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        // set the build quality to the release branch build quality
                        this.BuildQuality = this.ReleaseBranchBuildQuality;
                    }

                    // determine if the branch is a support branch
                    else if (!string.IsNullOrEmpty(this.SupportBranchBuildQuality)
                        && this.Branch.StartsWith(this.SupportBranchPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        // set the build quality to the support branch build quality
                        this.BuildQuality = this.SupportBranchBuildQuality;
                    }

                    // determine if the branch is a hotfix branch
                    else if (!string.IsNullOrEmpty(this.HotfixBranchBuildQuality)
                        && this.Branch.StartsWith(this.HotfixBranchPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        // set the build quality to the hotfix branch build quality
                        this.BuildQuality = this.HotfixBranchBuildQuality;
                    }
                }
            }

            // get the semantic version
            this.InformationalVersion = this.CurrentRelease;

            // determine if the prerelease tag is now set
            if (!string.IsNullOrEmpty(this.BuildQuality))
            {
                // append the build id
                this.PreReleaseTag = Invariant($"{this.BuildQuality}-{this.BuildId.PadLeft(5, '0')}");

                // determine if this is not a CI build
                if (!this.CI)
                {
                    // append the commit id
                    this.PreReleaseTag += Invariant($"-{commit.PadLeft(4, '0')}");
                }

                // append the prerelease tag to the informational version
                this.InformationalVersion += Invariant($"-{this.PreReleaseTag}");
            }

            // we are successful
            return true;
        }
        #endregion
    }
}
