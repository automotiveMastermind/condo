namespace PulseBridge.Condo.Build.Tasks
{
    using System;
    using System.Globalization;

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
        public string DateTimeUtc { get; set; }

        /// <summary>
        /// Executes the <see cref="GetVersionInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // determine if the date and time is set
            if (string.IsNullOrEmpty(this.DateTimeUtc))
            {
                // set the date and time to the current time
                this.DateTimeUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            }

            // define a variable to retain the date
            DateTime date;

            // attempt to parse the date
            if (!DateTime.TryParse(this.DateTimeUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date))
            {
                // could not parse; move on immediately
                return false;
            }

            // ensure that the date is always universal time
            date = date.ToUniversalTime();

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

            var build = date.ToString("yyddMM", CultureInfo.InvariantCulture);
            var commit = date.ToString("HHmm", CultureInfo.InvariantCulture);

            // determine if the commit id is not already set
            if (string.IsNullOrEmpty(this.CommitId))
            {
                // set the commit id
                this.CommitId = commit;
            }

            // determine if the build id is not already set
            if (string.IsNullOrEmpty(this.BuildId))
            {
                // set the build id
                this.BuildId = build;
            }

            // set the file version
            this.FileVersion = $"{version.Major}.{version.Minor}.{this.BuildId}.{commit}";

            // determine if the prerelease tag is not already set
            if (string.IsNullOrEmpty(this.BuildQuality))
            {
                // set the prelrease tag to alpha by default
                this.BuildQuality = "alpha";

                // only allow the prerelease tag to be set to anything other than alpha
                // on CI servers
                if (this.CI)
                {
                    // determine if the branch is a dev branch
                    if (this.Branch.StartsWith("dev", StringComparison.OrdinalIgnoreCase))
                    {
                        // set the prerelease tag to beta
                        this.BuildQuality = "beta";
                    }

                    // determine if this is a release branch
                    else if (this.Branch.StartsWith("release", StringComparison.OrdinalIgnoreCase)
                        || this.Branch.StartsWith("hotfix", StringComparison.OrdinalIgnoreCase))
                    {
                        // set the prerelease tag as a release candidate
                        this.BuildQuality = "rc";
                    }

                    // determine if the branch is master or main
                    else if (this.Branch.StartsWith("master", StringComparison.OrdinalIgnoreCase)
                        || this.Branch.StartsWith("main", StringComparison.OrdinalIgnoreCase))
                    {
                        // this should not have a prerelease tag
                        this.BuildQuality = null;
                    }
                }
            }

            // set the informational version to the semantic version
            this.InformationalVersion = this.SemanticVersion;

            // determine if the prerelease tag is now set
            if (!string.IsNullOrEmpty(this.BuildQuality))
            {
                // append the build id
                this.InformationalVersion += $"-{this.BuildQuality}-{this.BuildId.PadLeft(5,'0')}";

                // determine if this is not a CI build
                if (!this.CI)
                {
                    // append the commit id
                    this.InformationalVersion += $"-{commit.PadLeft(4, '0')}";
                }
            }

            // we are successful
            return true;
        }
    }
}