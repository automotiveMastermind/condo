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
        #region Properties
        /// <summary>
        /// Gets or sets the semantic version of the product.
        /// </summary>
        [Required]
        public string SemanticVersion { get; set; }

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
        /// Executes the <see cref="GetVersionInfo"/> task.
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
                Log.LogError("StartDateUtc property must be set.");

                // move on immediately
                return false;
            }

            // define a variable to retain the date
            DateTime now;

            // attempt to parse the date
            if (!DateTime.TryParse(this.BuildDateUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out now))
            {
                // log an error
                Log.LogError($"{nameof(BuildDateUtc)} property could not be parsed. Please verify that the date is valid.");

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
                Log.LogError($"{nameof(StartDateUtc)} property could not be parsed. Please verify that the date is valid.");

                // could not parse; move on immediately
                return false;
            }

            // ensure that the date is always universal time
            start = start.ToUniversalTime();

            // determine if the start date is after the current date
            if (start > now)
            {
                // log an error
                Log.LogError($"The {nameof(StartDateUtc)} cannot be after the {nameof(BuildDateUtc)}.");

                // start date is after now; move on immediately
                return false;
            }

            // define a variable to retain the version
            Version version;

            // attempt to parse the version
            if (!Version.TryParse(this.SemanticVersion, out version))
            {
                // emit an error if the version is not parsable
                this.Log.LogError($"Could not parse {nameof(this.SemanticVersion)} {this.SemanticVersion} -- it is not in the expected format.");

                // move on immediately
                return false;
            }

            // set the assembly version to the semantic version
            this.AssemblyVersion = version.ToString();

            // create a build number based on the number of years that have passed (and the current day of the year)
            var build = (now.Year - start.Year).ToString("D2") + now.DayOfYear.ToString("D3");

            // create a commit number from the current seconds
            var seconds = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
            var commit = seconds.ToString("D5", CultureInfo.InvariantCulture);

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

            // get the semantic version
            this.InformationalVersion = this.SemanticVersion;

            // determine if the prerelease tag is now set
            if (!string.IsNullOrEmpty(this.BuildQuality))
            {
                // append the build id
                this.PreReleaseTag = $"{this.BuildQuality}-{this.BuildId.PadLeft(5,'0')}";

                // determine if this is not a CI build
                if (!this.CI)
                {
                    // append the commit id
                    this.PreReleaseTag += $"-{commit.PadLeft(4, '0')}";
                }

                // append the prerelease tag to the informational version
                this.InformationalVersion += $"-{this.PreReleaseTag}";
            }

            // we are successful
            return true;
        }
        #endregion
    }
}