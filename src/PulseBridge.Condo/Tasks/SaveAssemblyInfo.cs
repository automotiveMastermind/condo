namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task used to save assembly info attributes to a specified path.
    /// </summary>
    public class SaveAssemblyInfo : Task
    {
        #region CONSTANTS
        private const string AttributeExpression = @"(\[assembly[\s:]+{0}\([\@\""]+)(.*)(\""\)\])";
        private const string MetadataExpression = @"(\[assembly[\s:]+AssemblyMetadata\([@\""]+{0}\""[\,\s]+[@\""]+)(.*)(\""\)\])";

        private const string AttributeFormat = @"[assembly: {0}(@""{1}"")]";
        private const string MetadataFormat = @"[assembly: AssemblyMetadata(""{0}"", @""{1}"")]";

        private const string ValueFormat = @"${{1}}{0}${{3}}";
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the assembly info path.
        /// </summary>
        [Required]
        public string AssemblyInfoPath { get; set; }

        /// <summary>
        /// Gets or sets the semantic version of the product.
        /// </summary>
        [Required]
        public string CurrentRelease { get; set; }

        /// <summary>
        /// Gets or sets the company responsible for the build.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        ///  Gets or sets the name of the product.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the branch of the build.
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        public string AssemblyVersion { get; private set; }

        /// <summary>
        /// Gets or sets the file version.
        /// </summary>
        public string FileVersion { get; private set; }

        /// <summary>
        /// Gets or sets the informational version of the build.
        /// </summary>
        public string InformationalVersion { get; private set; }

        /// <summary>
        /// Gets or sets the build quality of the build.
        /// </summary>
        public string BuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the build ID associated with the build.
        /// </summary>
        public string BuildId { get; set; }

        /// <summary>
        /// Gets or sets the commit ID associated with the build.
        /// </summary>
        public string CommitId { get; set; }

        /// <summary>
        /// Gets or sets the pull request ID associated with the build.
        /// </summary>
        public string PullRequestId { get; set; }

        /// <summary>
        /// Sets the date and time of the build.
        /// </summary>
        public string BuildDateUtc { get; set; }

        /// <summary>
        /// Gets the URI of the repository that is identified by the source control server.
        /// </summary>
        public string RepositoryUri { get; set; }

        /// <summary>
        /// Gets or sets the build URI of the build.
        /// </summary>
        public string BuildUri { get; set; }

        /// <summary>
        /// Gets or sets the URI of the team responsible for the build.
        /// </summary>
        public string TeamUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine or agent that is creating the build.
        /// </summary>
        public string BuildOn { get; set; }

        /// <summary>
        /// Gets or sets the name of the user that requested the build.
        /// </summary>
        public string BuildFor { get; set; }

        /// <summary>
        /// Gets or sets the name of the build.
        /// </summary>
        public string BuildName { get; set; }

        /// <summary>
        /// Gets or sets the platform on which the build was created.
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the build.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Gets or sets the authors responsible for the build.
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// Gets or sets the SPL of the license associated with the build.
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Gets or sets the URI of the license associated with the build.
        /// </summary>
        public string LicenseUri { get; set; }

        /// <summary>
        /// Gets or sets the copyright of the build.
        /// </summary>
        public string Copyright { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="SaveAssemblyInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task was successfully executed.
        /// </returns>
        public override bool Execute()
        {
            // capture the path
            var path = this.AssemblyInfoPath;

            // determine if the path exists
            if (string.IsNullOrEmpty(path))
            {
                // log an error
                this.Log.LogError("A path must be specified in order to save version info.");

                // move on immediately
                return false;
            }

            // define a variable to retain the version
            Version version;

            // attempt to parse the file version
            if (!Version.TryParse(this.CurrentRelease, out version))
            {
                // log an error
                this.Log.LogError("A semantic version must be supplied in order to save version info.");

                // move on immediately
                return false;
            }

            // log a message
            this.Log.LogMessage(MessageImportance.High, "{0,-19}: {1}", "Path", this.AssemblyInfoPath);

            // determine if the informational version is not specified
            if (string.IsNullOrEmpty(this.InformationalVersion))
            {
                // create an informational version which includes the revision number as a tag
                this.InformationalVersion = string.Format
                    (@"{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }

            // define a variable to retain the assembly info contents
            string contents = null;

            // determine if the file already exists
            if (File.Exists(path))
            {
                // read the contents of the file
                contents = File.ReadAllText(path).Trim();
            }

            // setermine if no contents exist
            if (string.IsNullOrEmpty(contents))
            {
                // create the directory
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                // create a string builder used to create the content
                var builder = new StringBuilder();

                // append the auto generated documentation header
                builder.AppendLine("//------------------------------------------------------------------------------");
                builder.AppendLine("// <auto-generated>");
                builder.AppendLine("// This code was generated by a tool.");
                builder.AppendLine("// </auto-generated>");
                builder.AppendLine("//------------------------------------------------------------------------------");
                builder.AppendLine();

                // add the reflection namespace
                builder.AppendLine("using System.Reflection;");
                builder.AppendLine();

                // save the builder to the contents
                contents = builder.ToString();
            }

            if (!string.IsNullOrEmpty(this.Company))
            {
                contents = this.AttributeReplace(contents, "AssemblyCompany", this.Company);
            }

            if (!string.IsNullOrEmpty(this.Product))
            {
                contents = this.AttributeReplace(contents, "AssemblyProduct", this.Product);
            }

            if (!string.IsNullOrEmpty(this.ProjectName))
            {
                contents = this.AttributeReplace(contents, "AssemblyTitle", this.ProjectName);
            }

            if (!string.IsNullOrEmpty(this.Copyright))
            {
                contents = this.AttributeReplace(contents, "AssemblyCopyright", this.Copyright);
            }

            if (!string.IsNullOrEmpty(this.AssemblyVersion))
            {
                contents = this.AttributeReplace(contents, "AssemblyVersion", this.AssemblyVersion + ".0");
            }

            if (!string.IsNullOrEmpty(this.FileVersion))
            {
                contents = this.AttributeReplace(contents, "AssemblyFileVersion", this.FileVersion);
            }

            if (!string.IsNullOrEmpty(this.InformationalVersion))
            {
                contents = this.AttributeReplace(contents, "AssemblyInformationalVersion", this.InformationalVersion);
            }

            if (!string.IsNullOrEmpty(this.Configuration))
            {
                contents = this.AttributeReplace(contents, "AssemblyConfiguration", this.Configuration);
            }

            contents = this.MetadataReplace(contents, "BuildDateUtc", this.BuildDateUtc);

            if (!string.IsNullOrEmpty(this.Platform))
            {
                contents = this.MetadataReplace(contents, "Platform", this.Platform);
            }

            if (!string.IsNullOrEmpty(this.Authors))
            {
                contents = this.MetadataReplace(contents, "Authors", this.Authors);
            }

            if (!string.IsNullOrEmpty(this.Branch))
            {
                contents = this.MetadataReplace(contents, "Branch", this.Branch);
            }

            if (!string.IsNullOrEmpty(this.BuildQuality))
            {
                contents = this.MetadataReplace(contents, "BuildQuality", this.BuildQuality);
            }

            if (!string.IsNullOrEmpty(this.BuildId))
            {
                contents = this.MetadataReplace(contents, "BuildId", this.BuildId);
            }

            if (!string.IsNullOrEmpty(this.CommitId))
            {
                contents = this.MetadataReplace(contents, "CommitId", this.CommitId);
            }

            if (!string.IsNullOrEmpty(this.PullRequestId))
            {
                contents = this.MetadataReplace(contents, "PullRequestId", this.PullRequestId);
            }

            if (!string.IsNullOrEmpty(this.BuildOn))
            {
                contents = this.MetadataReplace(contents, "BuildOn", this.BuildOn);
            }

            if (!string.IsNullOrEmpty(this.BuildFor))
            {
                contents = this.MetadataReplace(contents, "BuildFor", this.BuildFor);
            }

            if (!string.IsNullOrEmpty(this.BuildName))
            {
                contents = this.MetadataReplace(contents, "BuildName", this.BuildName);
            }

            if (!string.IsNullOrEmpty(this.TeamUri))
            {
                contents = this.MetadataReplace(contents, "TeamUri", this.TeamUri);
            }

            if (!string.IsNullOrEmpty(this.RepositoryUri))
            {
                contents = this.MetadataReplace(contents, "RepositoryUri", this.RepositoryUri);
            }

            if (!string.IsNullOrEmpty(this.BuildUri))
            {
                contents = this.MetadataReplace(contents, "BuildUri", this.BuildUri);
            }

            if (!string.IsNullOrEmpty(this.License))
            {
                contents = this.MetadataReplace(contents, "License", this.License);
            }

            if (!string.IsNullOrEmpty(this.LicenseUri))
            {
                contents = this.MetadataReplace(contents, "LicenseUri", this.LicenseUri);
            }

            File.WriteAllText(path, contents);

            return true;
        }

        private string AttributeReplace(string contents, string name, string value)
        {
            this.Log.LogMessage(MessageImportance.Low, "  {0,-17}: {1}", name, value);

            var expression = string.Format(AttributeExpression, name);
            var format = string.Format(ValueFormat, value);
            var regex = new Regex(expression);

            if (!regex.IsMatch(contents))
            {
                contents = string.Concat(contents, Environment.NewLine, string.Format(AttributeFormat, name, value));
                return contents;
            }

            return regex.Replace(contents, format, 1);
        }

        private string MetadataReplace(string contents, string name, string value)
        {
            this.Log.LogMessage(MessageImportance.Low, "  {0,-17}: {1}", name, value);

            var expression = string.Format(MetadataExpression, name);
            var format = string.Format(ValueFormat, value);
            var regex = new Regex(expression);

            if (!regex.IsMatch(contents))
            {
                contents = string.Concat(contents, Environment.NewLine, string.Format(MetadataFormat, name, value));
                return contents;
            }

            return regex.Replace(contents, format, 1);
        }
        #endregion
    }
}
