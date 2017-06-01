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
            // get the current version
            var current = this.gitlog.Versions.LastOrDefault();
            var currentVersion = current.Key ?? new SemanticVersion(major: 0, minor: 0, patch: 0);

            // get the last version of the same build quality
            var last = this.gitlog.Versions.LastOrDefault(v => !v.Key.IsPrerelease);
            var lastVersion = last.Key;
            var lastCommit = lastVersion == null ? null : this.gitlog.Tags.First(tag => lastVersion.Equals(tag.Version)).Hash;

            // get the version
            var version = lastVersion ?? currentVersion;

            // set the current version and current release properties
            this.CurrentVersion = version.ToNormalizedString();
            this.CurrentRelease = version.ToString("V", VersionFormatter.Instance);

            // get the level for the next version
            var level = this.RecommendLevel(lastCommit);

            // set the next release version
            this.NextRelease = RecommendRelease(version, level, releaseLabel: null);

            // set the recommended version
            this.RecommendedRelease = RecommendRelease(version, level, this.BuildQuality);

            // move on immediately
            return true;
        }

        private static string RecommendRelease(SemanticVersion version, int level, string releaseLabel)
        {
            if (version.Major == 0)
            {
                if (string.IsNullOrEmpty(releaseLabel))
                {
                    level = 0;
                }
                else if (level == 0)
                {
                    level = 1;
                }
            }

            switch (level)
            {
                case 0:
                    version = new SemanticVersion(version.Major + 1, 0, 0, releaseLabel);
                    break;
                case 1:
                    version = new SemanticVersion(version.Major, version.Minor + 1, 0, releaseLabel);
                    break;
                case 2:
                    version = new SemanticVersion(version.Major, version.Minor, version.Patch + 1, releaseLabel);
                    break;
                default:
                    version = new SemanticVersion(version.Major, version.Minor, version.Patch, releaseLabel);
                    break;
            }

            return version.ToString();
        }

        private int RecommendLevel(string hash)
        {
            // set the default bump level to patch
            var level = 2;

            // capture the commits
            var commits = this.gitlog.Commits;

            // set an index to start at the beginning of the commits (latest)
            var index = string.IsNullOrEmpty(hash) ? commits.Count - 1 : -1;

            // continue iterating until the last commit
            while (++index < commits.Count)
            {
                // determine if we have found the current version commit
                if (commits[index].Hash.Equals(hash))
                {
                    // break from the loop
                    break;
                }
            }

            // iterate starting at the latest version commit
            while (index-- > 0)
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
        #endregion
    }
}
