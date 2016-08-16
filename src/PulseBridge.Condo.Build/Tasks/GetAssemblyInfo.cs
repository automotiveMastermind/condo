namespace PulseBridge.Condo.Build.Tasks
{
    using System;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that accumulates assembly information.
    /// </summary>
    public class GetAssemblyInfo : Task
    {
        /// <summary>
        /// Gets or sets the semantic version of the product.
        /// </summary>
        [Required]
        public string SemanticVersion { get; set; }

        /// <summary>
        /// Gets or sets the branch used to determine the pre-release tag.
        /// </summary>
        public string Branch { get; set; } = "<unknown>";

        /// <summary>
        /// Gets or sets a value indicating whether or not the build is a CI build.
        /// </summary>
        public bool CI { get; set; } = false;

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
        public string PreReleaseTag { get; set; }

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
        public DateTime DateTimeUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Executes the <see cref="GetVersionInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // define a variable to retain the version
            Version version;

            // attempt to parse the version
            if (!Version.TryParse(this.SemanticVersion, out version))
            {
                // emit an error if the version is not parsable
                this.Log.LogError("Could not parse version {0} -- it is not in the expected format.", this.SemanticVersion);

                // move on immediately
                return false;
            }

            // set the assembly version to the semantic version
            this.AssemblyVersion = version.ToString();

            // determine if the commit id is not already set
            if (string.IsNullOrEmpty(this.CommitId))
            {
                // set the commit id
                this.CommitId = this.DateTimeUtc.ToString("HHmm");
            }

            // determine if the build id is not already set
            if (string.IsNullOrEmpty(this.BuildId))
            {
                // set the build id
                this.BuildId = this.DateTimeUtc.ToString("yyddMM");
            }

            // set the file version
            this.FileVersion = $"{version.Major}.{version.Minor}.{this.BuildId}.{this.CommitId}";

            // determine if the prerelease tag is not already set
            if (string.IsNullOrEmpty(this.PreReleaseTag))
            {
                // set the prelrease tag to alpha by default
                this.PreReleaseTag = "alpha";

                // only allow the prerelease tag to be set to anything other than alpha
                // on CI servers
                if (this.CI)
                {
                    // determine if the branch is a dev branch
                    if (this.Branch.StartsWith("dev", StringComparison.OrdinalIgnoreCase))
                    {
                        // set the prerelease tag to beta
                        this.PreReleaseTag = "beta";
                    }

                    // determine if this is a release branch
                    else if (this.Branch.StartsWith("release", StringComparison.OrdinalIgnoreCase)
                        || this.Branch.StartsWith("hotfix", StringComparison.OrdinalIgnoreCase))
                    {
                        // set the prerelease tag as a release candidate
                        this.PreReleaseTag = "rc";
                    }

                    // determine if the branch is master or main
                    else if (this.Branch.StartsWith("master", StringComparison.OrdinalIgnoreCase)
                        || this.Branch.StartsWith("main", StringComparison.OrdinalIgnoreCase))
                    {
                        // this should not have a prerelease tag
                        this.PreReleaseTag = null;
                    }
                }
            }

            // set the informational version to the semantic version
            this.InformationalVersion = this.SemanticVersion;

            // determine if the prerelease tag is now set
            if (!string.IsNullOrEmpty(this.PreReleaseTag))
            {
                // append the build id
                this.PreReleaseTag += $"-{this.BuildId}";

                // determine if this is not a CI build
                if (!this.CI)
                {
                    // append the commit id
                    this.PreReleaseTag += $"-{this.CommitId}";
                }

                // set the informational version
                this.InformationalVersion += $"-{this.PreReleaseTag}";
            }

            // we are successful
            return true;
        }
    }
}