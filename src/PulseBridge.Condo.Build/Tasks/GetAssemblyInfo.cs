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
        [Output]
        public string SemanticVersion { get; set; } = "1.0.0";

        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        [Output]
        public string AssemblyVersion { get; set; }

        /// <summary>
        /// Gets or sets the file version.
        /// </summary>
        [Output]
        public string FileVersion { get; set; }

        /// <summary>
        /// Gets or sets the pre-release tag.
        /// </summary>
        [Output]
        public string PreReleaseTag { get; set; }

        /// <summary>
        /// Gets or sets the informational version.
        /// </summary>
        [Output]
        public string InformationalVersion { get; set; }

        /// <summary>
        /// Gets or sets the branch used to determine the pre-release tag.
        /// </summary>
        [Output]
        public string Branch { get; set; } = "<unknown>";

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
        public DateTime UtcDateTime { private get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the name of the user or agent that requested the build.
        /// </summary>
        [Output]
        public string BuiltBy { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine that is executing the build.
        /// </summary>
        [Output]
        public string BuiltOn { get; set; }

        /// <summary>
        /// Gets or sets the URI of the team responsible for the build.
        /// </summary>
        [Output]
        public string TeamUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the project associated with the build.
        /// </summary>
        [Output]
        public string TeamProject { get; set; }

        /// <summary>
        /// Gets or sets the name of the build on the continuous integration server.
        /// </summary>
        [Output]
        public string BuildName { get; set; }

        /// <summary>
        /// Gets or sets the URI of the repository containing the code that represents the build.
        /// </summary>
        [Output]
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets or sets the URI for the build on a continuous integration server.
        /// </summary>
        /// <returns></returns>
        [Output]
        public string BuildUri { get; set; }

        /// <summary>
        /// Gets or sets the surrogate identifier of the pull request that is associated with the build.
        /// </summary>
        [Output]
        public string PullRequestId { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider hosting the repository.
        /// </summary>
        [Output]
        public string RepositoryProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the product associated with the build.
        /// </summary>
        [Output]
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the name of the company that owns the product.
        /// </summary>
        [Output]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the copyright notice for the build.
        /// </summary>
        [Output]
        public string Copyright { get; set; }

        /// <summary>
        /// Gets or sets the SPL of the license associated with the build.
        /// </summary>
        [Output]
        public string License { get; set; }

        /// <summary>
        /// Gets or sets the URI for the license of the build.
        /// </summary>
        [Output]
        public string LicenseUri { get; set; }

        /// <summary>
        /// Executes the <see cref="GetVersionInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(this.AssemblyVersion))
            {
                // set the assembly version to the semantic version
                this.AssemblyVersion = this.SemanticVersion;
            }

            Version version;

            if (!Version.TryParse(this.SemanticVersion, out version))
            {
                this.Log.LogError("Could not parse version {0} -- it is not in the expected format.", this.SemanticVersion);

                return false;
            }

            if (string.IsNullOrEmpty(this.CommitId))
            {
                this.CommitId = this.UtcDateTime.ToString("HHmm");
            }

            if (string.IsNullOrEmpty(this.BuildId))
            {
                this.BuildId = this.UtcDateTime.ToString("yyddMM");
            }

            // set the file version
            this.FileVersion = $"{version.Major}.{version.Minor}.{this.BuildId}.{this.CommitId}";

            return true;
        }
    }
}