namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using Newtonsoft.Json;
    using NuGet.Versioning;

    /// <summary>
    /// Represents a Microsoft Build task used to update project.json files to a specific semantic version.
    /// </summary>
    public class UpdateProjectJson : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the collection of projects to update.
        /// </summary>
        [Required]
        public ITaskItem[] Projects { get; set; }

        /// <summary>
        /// Gets or sets the version to set within the project.json files.
        /// </summary>
        [Required]
        public string Version { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="UpdateProjectJson"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task was successful.
        /// </returns>
        public override bool Execute()
        {
            SemanticVersion version;

            if (!SemanticVersion.TryParse(this.Version, out version))
            {
                this.Log.LogError($"The supplied version of {this.Version} is not a valid semantic version.");
            }

            // normalize the version
            version = new SemanticVersion(version.Major, version.Minor, version.Patch);

            try
            {
                // iterate over each project file
                foreach (var project in this.Projects)
                {
                    // read the content
                    var content = File.ReadAllText(project.ItemSpec);

                    // get the project
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                    // change the version
                    dictionary["version"] = version.ToString();

                    // serialize the content
                    content = JsonConvert.SerializeObject(dictionary, Formatting.Indented);

                    // save the file back
                    File.WriteAllText(project.ItemSpec, content);

                    // log a message
                    this.Log.LogMessage(MessageImportance.High, $"Updated {project.ItemSpec} to version {version}");
                }
            }
            catch (Exception netEx)
            {
                // log an error
                Log.LogErrorFromException(netEx);

                // move on immediately
                return false;
            }

            // assume success
            return true;
        }
        #endregion
    }
}
