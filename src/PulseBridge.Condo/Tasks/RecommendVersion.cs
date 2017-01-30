namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using NuGet.Versioning;

    /// <summary>
    /// Represents a Microsoft Build task used to recommend a semantic version based on a commit history.
    /// </summary>
    public class RecommendVersion : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the current (latest) version.
        /// </summary>
        public string LatestVersion { get; set; }

        /// <summary>
        /// Gets or sets the latest commit included in the current (latest) version.
        /// </summary>
        public string LatestVersionCommit { get; set; }

        /// <summary>
        /// Gets or sets the current branch.
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the master branch.
        /// </summary>
        public string MasterBranch { get; set; }

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
        /// Gets the header correspondence field used to detect a feature (minor) bump.
        /// </summary>
        public string MinorCorrespondence { get; set; }

        /// <summary>
        /// Gets the header correspondence value used to detect a feature (minor) bump.
        /// </summary>
        public string MinorValue { get; set; }
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(this.LatestVersion))
            {
                this.LatestVersion = "0.0.0";
            }

            var version = default(SemanticVersion);

            if (!SemanticVersion.TryParse(this.LatestVersion, out version))
            {
                Log.LogError($"The version ({LatestVersion}) is invalid.");

                return false;
            }

            // normalize the version
            version = new SemanticVersion(version.Major, version.Minor, version.Patch);

            this.CurrentRelease = version.ToString();

            var log = GetCommitInfo.GitLog;

            if (log == null)
            {
                Log.LogError($"You must call the {nameof(GetCommitInfo)} task before calling this task.");

                return false;
            }

            if (version.Major == 0 && this.Branch.Equals(this.MasterBranch, StringComparison.OrdinalIgnoreCase))
            {
                // set the version to 1.0.0
                this.SetVersion(version, level: 0);

                // move on immediately
                return true;
            }

            var level = 2;

            var commits = log.Commits;
            var index = 0;

            for (index = commits.Count - 1; index > 0; index--)
            {
                if (commits[index].Hash.Equals(this.LatestVersionCommit))
                {
                    break;
                }
            }

            for (index = index + 1; index < commits.Count; index++)
            {
                var commit = commits[index];

                if (commit.Notes.Count > 0)
                {
                    level = 0;
                    break;
                }

                if (commit.HeaderCorrespondence
                    .Any(h => h.Key.Equals(this.MinorCorrespondence, StringComparison.OrdinalIgnoreCase)
                        && h.Value.Equals(this.MinorValue, StringComparison.OrdinalIgnoreCase)))
                {
                    level = 1;
                }
            }

            // determine if this is a development version that has not yet been released
            if (version.Major == 0 && level == 0)
            {
                // down-level to 1
                level = 1;
            }

            // set the version
            this.SetVersion(version, level);

            // move on immediately
            return true;
        }

        private void SetVersion(SemanticVersion version, int level)
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

            this.NextRelease = version.ToString();
        }
        #endregion
    }
}
