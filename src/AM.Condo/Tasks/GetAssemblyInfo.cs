// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAssemblyInfo.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Globalization;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Versioning;

    using static System.FormattableString;

    /// <summary>
    /// Represents a Microsoft Build task that accumulates assembly information.
    /// </summary>
    public class GetAssemblyInfo : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the semantic version of the product.
        /// </summary>
        [Required]
        public string RecommendedRelease { get; set; }

        /// <summary>
        /// Gets or sets the date and time (UTC) that the project was first started.
        /// </summary>
        [Required]
        public string StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the pre-release tag.
        /// </summary>
        public string BuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the suffix tag.
        /// </summary>
        public string SuffixTag { get; set; }

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
        /// Gets the major version number of the release.
        /// </summary>
        [Output]
        public string MajorVersion { get; private set; }

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        [Output]
        public string AssemblyVersion { get; private set; }

        /// <summary>
        /// Gets the file version.
        /// </summary>
        [Output]
        public string FileVersion { get; private set; }

        /// <summary>
        /// Gets the informational version.
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
        /// Gets or sets the date and time used to determine the version.
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
                this.Log.LogError(Invariant
                    ($"{nameof(this.StartDateUtc)} property must be set."));

                // move on immediately
                return false;
            }

            // define a variable to retain the date

            // attempt to parse the date
            if (!DateTime.TryParse(this.BuildDateUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime now))
            {
                // log an error
                this.Log.LogError
                    (Invariant($"{nameof(this.BuildDateUtc)} property could not be parsed. Please verify that the date is valid."));

                // could not parse; move on immediately
                return false;
            }

            // ensure that the date is always universal time
            now = now.ToUniversalTime();

            // define a variable to retain the start date

            // attempt to parse the start date
            if (!DateTime.TryParse(this.StartDateUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime start))
            {
                // log an error
                this.Log.LogError
                    (Invariant($"{nameof(this.StartDateUtc)} property could not be parsed. Please verify that the date is valid."));

                // could not parse; move on immediately
                return false;
            }

            // ensure that the date is always universal time
            start = start.ToUniversalTime();

            // determine if the start date is after the current date
            if (start > now)
            {
                // log an error
                this.Log.LogError
                    (Invariant($"The {nameof(this.StartDateUtc)} cannot be after the {nameof(this.BuildDateUtc)}."));

                // start date is after now; move on immediately
                return false;
            }

            // attempt to parse the version
            if (!SemanticVersion.TryParse(this.RecommendedRelease, out SemanticVersion version))
            {
                // emit an error if the version is not parsable
                this.Log.LogError(Invariant($"Could not parse {nameof(this.RecommendedRelease)} {this.RecommendedRelease} -- it is not in the expected format."));

                // move on immediately
                return false;
            }

            // set the assembly version to the semantic version
            this.InformationalVersion = this.AssemblyVersion = version.ToString("x.y.z", VersionFormatter.Instance);

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
            this.FileVersion = version.ToString($"x.y.{this.BuildId}.{commit}", VersionFormatter.Instance);

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

            // determine if the suffix tag is now set
            if (!string.IsNullOrEmpty(this.SuffixTag))
            {
                // apppend the suffix tag to the informational version
                this.InformationalVersion += Invariant($"-{this.SuffixTag}");
            }

            // set the major version
            this.MajorVersion = version.Major.ToString();

            // we are successful
            return true;
        }
        #endregion
    }
}
