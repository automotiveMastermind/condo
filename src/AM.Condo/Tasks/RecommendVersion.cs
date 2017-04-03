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
        public string BuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the header correspondence field used to detect a feature (minor) bump.
        /// </summary>
        public string MinorCorrespondence { get; set; }

        /// <summary>
        /// Gets or sets the header correspondence value used to detect a feature (minor) bump.
        /// </summary>
        public string MinorValue { get; set; }

        /// <summary>
        /// Gets or sets the current (latest) version.
        /// </summary>
        [Output]
        public string CurrentVersion { get; set; }

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
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Execute()
        {
            // determine if no version tags exist
            if (!this.gitlog.Versions.Any())
            {
                // assume this is the very first release and set to all zeros
                this.CurrentVersion = this.CurrentRelease = "0.0.0";

                // set the recommended release to the first patch
                this.RecommendedRelease = "0.0.1";

                // set the next release to the current major release
                this.NextRelease = "1.0.0";

                // move on immediately
                return true;
            }

            // get the current version
            var current = this.gitlog.Versions.Last();
            var currentVersion = current.Key;
            var currentCommit = this.gitlog.Tags.First(tag => currentVersion.Equals(tag.Version)).Hash;

            // set the current version and current release properties
            this.CurrentVersion = currentVersion.ToNormalizedString();
            this.CurrentRelease = currentVersion.ToString("V", VersionFormatter.Instance);

            // get the level for the next version
            var level = this.RecommendLevel(currentVersion, currentCommit);

            // set the next release version
            this.NextRelease = RecommendRelease(currentVersion, currentVersion.Major == 0 ? 0 : level);

            // parse the prerelease build quality
            var currentBuildQuality = currentVersion.Release.Split('-').FirstOrDefault();

            // get the comparison
            var comparison = string.Compare(this.BuildQuality, currentBuildQuality);

            // determine if the current build quality is greater than the new build quality
            if (string.IsNullOrEmpty(currentBuildQuality) || string.Compare(this.BuildQuality, currentBuildQuality) < 0)
            {
                // set the recommended release to the next release
                this.RecommendedRelease = RecommendRelease(currentVersion, string.IsNullOrEmpty(this.BuildQuality) ? level : 2);

                // move on immediately
                return true;
            }

            // we should not increment
            this.RecommendedRelease = this.CurrentRelease;

            // move on immediately
            return true;
        }

        private int RecommendLevel(SemanticVersion version, string hash)
        {
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
                if (commits[index].Hash.Equals(hash))
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

            // return the level
            return level;
        }

        private static string RecommendRelease(SemanticVersion version, int level)
        {
            switch (level)
            {
                case 0:
                    version = new SemanticVersion(version.Major + 1, 0, 0);
                    break;
                case 1:
                    version = new SemanticVersion(version.Major, version.Minor + 1, 0);
                    break;
                case 2:
                    version = new SemanticVersion(version.Major, version.Minor, version.Patch + 1);
                    break;
                default:
                    version = new SemanticVersion(version.Major, version.Minor, version.Patch);
                    break;
            }

            return version.ToString();
        }
        #endregion
    }
}
