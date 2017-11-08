// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetDockerMetadata.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Newtonsoft.Json;
    using NuGet.Versioning;

    /// <summary>
    /// Represents a Microsoft Build task used to set additional project metadata for docker files.
    /// </summary>
    public class GetDockerMetadata : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the list of dockerfiles for which to set additional metadata.
        /// </summary>
        [Required]
        [Output]
        public ITaskItem[] Dockerfiles { get; set; }

        /// <summary>
        /// Gets or sets the product name to use when only a single project is present.
        /// </summary>
        [Required]
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the root path of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the build quality associated with the tag.
        /// </summary>
        public string BuildQuality { get; set; }

        /// <summary>
        /// Gets or sets the informational version associated with the tag.
        /// </summary>
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to enable extended tags.
        /// </summary>
        public bool EnableExtendedTags { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetDockerMetadata"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task was successfully executed.
        /// </returns>
        public override bool Execute()
        {
            // iterate over each project
            foreach (var project in this.Dockerfiles)
            {
                // set the metadata on the project
                this.SetMetadata(project);

                // log a message
                this.Log.LogMessage
                    (
                        MessageImportance.Low,
                        $"Updated metadata for dockerfile: {project.GetMetadata("DockerName")}"
                    );
            }

            // assume its always true
            return true;
        }

        private void SetMetadata(ITaskItem dockerfile)
        {
            // get the full path of the project file
            var path = dockerfile.GetMetadata("FullPath");

            // get the directory name from the path
            var directory = Path.GetDirectoryName(path);
            var parent = Path.GetDirectoryName(directory);
            var group = Path.GetFileName(directory);

            // set the docker name to the product name by default
            var dockerName = this.Product;

            // determine if the project is rooted
            var rooted = string.Equals
            (
                Path.GetFullPath(directory + Path.DirectorySeparatorChar),
                Path.GetFullPath(this.RepositoryRoot + Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase
            );

            // determine if we are not rooted and the group is a well-known path
            if (!rooted && !GetProjectMetadata.WellKnownFolders.Contains(group, StringComparer.OrdinalIgnoreCase))
            {
                // set the project name to the group
                dockerName = group;

                // use the parent of the group folder, which means multiple projects are contained within the folder
                group = Path.GetFileName(parent);
            }

            // get the docker file path
            var projectFile = Directory.GetFiles(directory, "*.*proj").FirstOrDefault();

            // determine if the project file existed
            if (!string.IsNullOrEmpty(projectFile))
            {
                // use the name of the project as the docker name
                dockerName = Path.GetFileNameWithoutExtension(projectFile);
            }

            // calculate the label
            dockerName = dockerName.ToLower();

            // set the project name for docker
            dockerfile.SetMetadata("Label", dockerName);

            // parse the stupid version
            var version = SemanticVersion.Parse(this.Version);

            // create a dictionary to track tags
            var tags = new Dictionary<string, string>();
            var labels = new Dictionary<string, string>();

            // add the full version tag
            tags.Add("VersionTag", $"{this.Version}");
            labels.Add("VersionLabel", $"{dockerName}:{this.Version}");

            // determine if we should calculate extended tags
            if (this.EnableExtendedTags)
            {
                // determine if the build quality is set
                if (string.IsNullOrEmpty(this.BuildQuality))
                {
                    tags.Add("LatestTag", "latest");                            // :latest
                    labels.Add("LatestLabel", $"{dockerName}:latest");

                    tags.Add("StableTag", "stable");                            // :stable
                    labels.Add("StableLabel", $"{dockerName}:stable");

                    tags.Add("MajorTag", $"{version.Major}");                   // :1
                    labels.Add("MajorLabel", $"{dockerName}:{version.Major}");

                    tags.Add("MinorTag", $"{version.Major}-{version.Minor}");   // :1.1
                    labels.Add("MinorLabel", $"{dockerName}:{version.Major}-{version.Minor}");
                }
                else
                {
                    tags.Add("BuildQualityTag", this.BuildQuality);             // :beta
                    labels.Add("BuildQualityLabel", $"{dockerName}:{this.BuildQuality}");

                    tags.Add("PrereleaseTag", "prerelease");                    // :prerelease
                    labels.Add("PrereleaseLabel", $"{dockerName}:prerelease");

                    tags.Add("MajorTag", $"{version.Major}-{this.BuildQuality}");
                    labels.Add("MajorLabel", $"{dockerName}:{version.Major}-{this.BuildQuality}");

                    tags.Add("MinorTag", $"{version.Major}-{version.Minor}-{this.BuildQuality}");
                    labels.Add("MinorLabel", $"{dockerName}:{version.Major}-{version.Minor}-{this.BuildQuality}");
                }
            }

            // get the extension (platform) of the dockerfile
            var extension = Path.GetExtension(path);

            // determine if the platform is specified
            var hasPlatform = !string.IsNullOrEmpty(extension);

            // determine if the platform was specified
            if (hasPlatform)
            {
                // strip the extension from the substring
                extension = extension.Substring(1);
            }

            // set the metadata for the docker tags
            dockerfile.SetMetadata("DockerTags", string.Join(";", tags.Select(tag => tag.Value)));
            dockerfile.SetMetadata("DockerLabels", string.Join(";", labels.Select(label => label.Value)));

            // iterate over each tag
            foreach (var tag in tags)
            {
                // append the platform to the tag
                var value = hasPlatform ? $"{tag.Value}-{extension}" : tag.Value;

                // set the metadata
                dockerfile.SetMetadata(tag.Key, value);
            }

            // iterate over each tag
            foreach (var label in labels)
            {
                // append the platform to the tag
                var value = hasPlatform ? $"{label.Value}-{extension}" : label.Value;

                // set the metadata
                dockerfile.SetMetadata(label.Key, value);
            }
        }
        #endregion
    }
}
