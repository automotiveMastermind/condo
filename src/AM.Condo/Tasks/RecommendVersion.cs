// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecommendVersion.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Linq;

    using AM.Condo.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using NuGet.Versioning;

    /// <summary>
    /// Represents a Microsoft Build task used to recommend a semantic version based on a commit history.
    /// </summary>
    public class RecommendVersion : Task
    {
        #region Fields
        private readonly GitLog gitlog;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendVersion"/> class.
        /// </summary>
        public RecommendVersion()
            : this(GetCommitInfo.GitLog)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendVersion"/> class.
        /// </summary>
        /// <param name="gitlog">
        /// The git log that contains the versions and commits used to recommend the next version.
        /// </param>
        public RecommendVersion(GitLog gitlog)
        {
            // set the log
            this.gitlog = gitlog ?? throw new ArgumentNullException
                    (nameof(gitlog), $"You must call the {nameof(GetCommitInfo)} task before calling this task.");
        }
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the build quality of the release.
        /// </summary>
        [Required]
        public string BuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the build quality for the release branch.
        /// </summary>
        [Required]
        public string ReleaseBranchBuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the current (latest) version.
        /// </summary>
        public string LatestVersion { get; set; }

        /// <summary>
        /// Gets or sets the latest commit included in the current (latest) version.
        /// </summary>
        public string LatestVersionCommit { get; set; }

        /// <summary>
        /// Gets or sets the current semantic version.
        /// </summary>
        [Output]
        public string CurrentRelease { get; set; }

        /// <summary>
        /// Gets or sets the recommended version.
        /// </summary>
        [Output]
        public string NextRelease { get; set; }

        /// <summary>
        /// Gets or sets the current semantic version.
        /// </summary>
        [Output]
        public string RecommendedRelease { get; set; }

        /// <summary>
        /// Gets or sets the header correspondence field used to detect a feature (minor) bump.
        /// </summary>
        public string MinorCorrespondence { get; set; }

        /// <summary>
        /// Gets or sets the header correspondence value used to detect a feature (minor) bump.
        /// </summary>
        public string MinorValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not to force the initial release.
        /// </summary>
        public bool ShouldForceRelease => string.IsNullOrEmpty(this.BuildQuality)
            || this.BuildQuality.Equals(this.ReleaseBranchBuildQuality, StringComparison.OrdinalIgnoreCase);
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Execute()
        {
            // determine if the latest version is specified
            if (string.IsNullOrEmpty(this.LatestVersion))
            {
                // assume this is the very first release and set to all zeros
                this.LatestVersion = "0.0.0";
            }

            // define a variable to retain the version
            if (!SemanticVersion.TryParse(this.LatestVersion, out SemanticVersion version))
            {
                // log the error
                this.Log.LogError($"The version ({this.LatestVersion}) is invalid.");

                // move on immediately
                return false;
            }

            // determine if this is the first release
            if (version.Major == 0)
            {
                // set the current release
                this.CurrentRelease = new SemanticVersion(version.Major, version.Minor, version.Patch).ToString();

                // bump to the next minor bit for current and recommended release
                this.RecommendedRelease
                    = GetNextVersion(version, level: this.ShouldForceRelease ? 0 : 1);

                // set the next relese to the first major version
                this.NextRelease = "1.0.0";

                // move on immediately
                return true;
            }

            // determine if this is another build of the first release on the release or production branch
            if (version.Major == 1
                && version.Minor == 0
                && version.Patch == 0
                && version.IsPrerelease
                && this.ShouldForceRelease)
            {
                // set the current release
                this.CurrentRelease = this.RecommendedRelease = this.NextRelease = "1.0.0";

                // move on immediately
                return true;
            }

            // determine if the version is a release branch build
            var isReleaseBuildQuality = version.IsPrerelease
                && version.Release.StartsWith(this.ReleaseBranchBuildQuality, StringComparison.OrdinalIgnoreCase);

            // normalize the version
            version = new SemanticVersion(version.Major, version.Minor, version.Patch);

            // set the current release to the normalized version
            this.CurrentRelease = version.ToString();

            // get the last release version
            var last = this.gitlog.Versions.LastOrDefault(release => !release.Key.IsPrerelease);

            // determine if the last is not null
            if (last.Key != null)
            {
                // get the version from the key
                version = last.Key;

                // set the latest version and commit
                this.LatestVersion = version.ToFullString();
                this.LatestVersionCommit = last.Value.LastOrDefault().Hash;

                // normalize the version
                version = new SemanticVersion(version.Major, version.Minor, version.Patch);
            }
            else if (isReleaseBuildQuality)
            {
                // reduce the major version by one
                version = new SemanticVersion(version.Major - 1, version.Minor, version.Minor);

                // log a message
                this.Log.LogMessage($"The current release {version} is a release candidate, which does not yet exist on the current branch. The version will not be incremented.");
            }
            else
            {
                // create a normalized version that is the previous major version
                version = new SemanticVersion(version.Major > 1 ? version.Major - 1 : 1, 0, 0);

                // write a warning
                this.Log.LogWarning($"The current release is {version}, which is >= 1.0.0 but all previous releases have a prerelease tag. Falling back to previous major release version.");
            }

            // set the default bump level to patch
            var level = 2;

            // capture the commits
            var commits = this.gitlog.Commits;

            // set an index to start at the beginning of the commits
            var index = 0;

            // continue iterating until the last commit
            for (index = commits.Count - 1; index > 0; index--)
            {
                // determine if we have found the current version commit
                if (commits[index].Hash.Equals(this.LatestVersionCommit))
                {
                    // break from the loop
                    break;
                }
            }

            // iterate starting at the latest version commit
            for (index = index + 1; index < commits.Count; index++)
            {
                // capture the commit at that index
                var commit = commits[index];

                // determine if any notes (breaking changes) were discovered
                if (commit.Notes.Count > 0)
                {
                    // set the level to major
                    level = 0;

                    // break from the loop (we cant go any higher)
                    break;
                }

                // determine if the commit contains a minor correspondence value (feature)
                if (commit.HeaderCorrespondence
                    .Any(h => h.Key.Equals(this.MinorCorrespondence, StringComparison.OrdinalIgnoreCase)
                        && h.Value.Equals(this.MinorValue, StringComparison.OrdinalIgnoreCase)))
                {
                    // set the level to minor
                    level = 1;
                }
            }

            // set the next release version
            this.RecommendedRelease = this.NextRelease = GetNextVersion(version, level);

            // move on immediately
            return true;
        }

        private static string GetNextVersion(SemanticVersion version, int level)
        {
            switch (level)
            {
                case 0:
                    version = new SemanticVersion(version.Major + 1, 0, 0);
                    break;
                case 1:
                    version = new SemanticVersion(version.Major, version.Minor + 1, 0);
                    break;
                default:
                    version = new SemanticVersion(version.Major, version.Minor, version.Patch + 1);
                    break;
            }

            return version.ToString();
        }
        #endregion
    }
}
