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
    using AM.Condo.Resources;
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

            // set the project dir
            dockerfile.SetMetadata("ProjectDir", directory);

            // determine if the project is rooted
            var rooted = string.Equals
            (
                Path.GetFullPath(directory + Path.DirectorySeparatorChar),
                Path.GetFullPath(this.RepositoryRoot + Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase
            );

            // try to figure out what the docker name should be (default to the product name if no alternative is available)
            var name = this.SetDockerName(directory);

            // determine if we are not rooted and the folder name is not a well-known folder
            if (!rooted && !GetProjectMetadata.WellKnownFolders.Contains(group, StringComparer.OrdinalIgnoreCase))
            {
                // determine if the name is equal to the product name
                if (name.Equals(this.Product, StringComparison.Ordinal))
                {
                    // set the project name to the current group (current directory name) to avoid collisions
                    name = group;
                }

                // use the parent of the group folder, which means multiple projects are contained within the folder
                group = Path.GetFileName(parent);
            }

            // set the label to lowercase
            name = name.ToLowerInvariant();

            // set the project name for docker
            dockerfile.SetMetadata("Label", name);

            // get the extension (platform) of the dockerfile
            var extension = Path.GetExtension(path);
            var tagSuffix = string.Empty;
            var latest = false;

            // determine if the extension is not null or empty and ends with latest
            if (!string.IsNullOrEmpty(extension)
                && (latest = extension.Equals(".latest", StringComparison.OrdinalIgnoreCase)))
            {
                // get the real extension
                extension = Path.GetExtension(Path.GetFileNameWithoutExtension(path));
            }

            // determine if the extension is still not null or empty
            if (!string.IsNullOrEmpty(extension))
            {
                // set the tag suffix
                tagSuffix = $"-{extension.Substring(1)}";
            }

            // parse the stupid version
            var version = SemanticVersion.Parse(this.Version);

            // create a dictionary to track tags
            var tags = new Dictionary<string, string>();
            var labels = new Dictionary<string, string>();

            // add the full version tag
            tags.Add("VersionTag", $"{this.Version}{tagSuffix}");
            labels.Add("VersionLabel", $"{name}:{this.Version}{tagSuffix}");

            // determine if we should calculate extended tags
            if (this.EnableExtendedTags)
            {
                // determine if the build quality is set
                if (string.IsNullOrEmpty(this.BuildQuality))
                {
                    tags.Add("LatestTagPlatform", $"latest{tagSuffix}");
                    labels.Add("LatestLabelPlatform", $"{name}:latest{tagSuffix}");

                    tags.Add("StableTagPlatform", $"stable{tagSuffix}");
                    labels.Add("StableLabelPlatform", $"{name}:stable{tagSuffix}");

                    tags.Add("MajorTagPlatform", $"{version.Major}{tagSuffix}");
                    labels.Add("MajorLabelPlatform", $"{name}:{version.Major}{tagSuffix}");

                    tags.Add("MinorTagPlatform", $"{version.Major}-{version.Minor}{tagSuffix}");
                    labels.Add("MinorLabelPlatform", $"{name}:{version.Major}-{version.Minor}{tagSuffix}");

                    // determine if this is also the latest marker
                    if (latest)
                    {
                        tags.Add("LatestTag", $"latest");
                        labels.Add("LatestLabel", $"{name}:latest");

                        tags.Add("StableTag", $"stable");
                        labels.Add("StableLabel", $"{name}:stable");

                        tags.Add("MajorTag", $"{version.Major}");
                        labels.Add("MajorLabel", $"{name}:{version.Major}");

                        tags.Add("MinorTag", $"{version.Major}-{version.Minor}");
                        labels.Add("MinorLabel", $"{name}:{version.Major}-{version.Minor}");
                    }
                }
                else
                {
                    tags.Add("BuildQualityTagPlatform", $"{this.BuildQuality}{tagSuffix}");
                    labels.Add("BuildQualityLabelPlatform", $"{name}:{this.BuildQuality}{tagSuffix}");

                    tags.Add("PrereleaseTagPlatform", $"prerelease{tagSuffix}");
                    labels.Add("PrereleaseLabelPlatform", $"{name}:prerelease{tagSuffix}");

                    tags.Add("MajorTagPlatform", $"{version.Major}-{this.BuildQuality}{tagSuffix}");
                    labels.Add("MajorLabelPlatform", $"{name}:{version.Major}-{this.BuildQuality}{tagSuffix}");

                    tags.Add("MinorTagPlatform", $"{version.Major}-{version.Minor}-{this.BuildQuality}{tagSuffix}");
                    labels.Add("MinorLabelPlatform", $"{name}:{version.Major}-{version.Minor}-{this.BuildQuality}{tagSuffix}");

                    // determine if this is also the latest marker
                    if (latest)
                    {
                        tags.Add("BuildQualityTag", $"{this.BuildQuality}");
                        labels.Add("BuildQualityLabel", $"{name}:{this.BuildQuality}");

                        tags.Add("PrereleaseTag", $"prerelease");
                        labels.Add("PrereleaseLabel", $"{name}:prerelease");

                        tags.Add("MajorTag", $"{version.Major}-{this.BuildQuality}");
                        labels.Add("MajorLabel", $"{name}:{version.Major}-{this.BuildQuality}");

                        tags.Add("MinorTag", $"{version.Major}-{version.Minor}-{this.BuildQuality}");
                        labels.Add("MinorLabel", $"{name}:{version.Major}-{version.Minor}-{this.BuildQuality}");
                    }
                }
            }

            // set the metadata for the docker tags
            dockerfile.SetMetadata("DockerTags", string.Join(";", tags.Select(tag => tag.Value)));
            dockerfile.SetMetadata("DockerLabels", string.Join(";", labels.Select(label => label.Value)));

            foreach (var tag in tags)
            {
                dockerfile.SetMetadata(tag.Key, tag.Value);
            }

            foreach (var label in labels)
            {
                dockerfile.SetMetadata(label.Key, label.Value);
            }
        }

        private string SetDockerName(string directory)
        {
            // get the docker file path
            var projectFile = Directory.EnumerateFiles(directory, "*.*proj").FirstOrDefault();

            // determine if the project file existed
            if (!string.IsNullOrEmpty(projectFile))
            {
                // use the name of the project as the docker name
                return Path.GetFileNameWithoutExtension(projectFile);
            }

            // get the project file path for package.json
            projectFile = Directory.EnumerateFiles(directory, "package.json").FirstOrDefault();

            // determine if the project file existed
            if (!string.IsNullOrEmpty(projectFile))
            {
                // read the json
                var json = File.ReadAllText(projectFile);

                // serialize as node project
                var name = JsonConvert.DeserializeObject<NodeProject>(json)?.Name;

                // determine if the name was found
                if (!string.IsNullOrEmpty(name))
                {
                    // return the name
                    return name;
                }
            }

            // get the project file path for package.json
            projectFile = Directory.EnumerateFiles(directory, "bower.json").FirstOrDefault();

            // determine if the project file existed
            if (!string.IsNullOrEmpty(projectFile))
            {
                // read the json
                var json = File.ReadAllText(projectFile);

                // serialize as bower project
                var name = JsonConvert.DeserializeObject<BowerProject>(json)?.Name;

                // determine if the name was found
                if (!string.IsNullOrEmpty(name))
                {
                    // return the name
                    return name;
                }
            }

            // use the product name
            return this.Product;
        }
        #endregion
    }
}
