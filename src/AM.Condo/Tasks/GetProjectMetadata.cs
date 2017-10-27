// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProjectMetadata.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a Microsoft Build task used to set additional project metadata for .NET CoreCLR projects using the
    /// project.json format.
    /// </summary>
    public class GetProjectMetadata : Task
    {
        #region Private Fields
        private static readonly string[] WellKnownFolders = { "src", "test", "docs", "samples" };

        private readonly List<ITaskItem> projects = new List<ITaskItem>();
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the list of projects for which to set additional metadata.
        /// </summary>
        [Required]
        [Output]
        public ITaskItem[] Projects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether restore is enabled.
        /// </summary>
        public bool Restore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not build is enabled.
        /// </summary>
        public bool Build { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not pack is enabled.
        /// </summary>
        public bool Pack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not test is enabled.
        /// </summary>
        public bool Test { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not publish is enabled.
        /// </summary>
        public bool Publish { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetProjectMetadata"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task was successfully executed.
        /// </returns>
        public override bool Execute()
        {
            // iterate over each project
            foreach (var project in this.Projects)
            {
                // set the metadata on the project
                this.SetMetadata(project);

                // log a message
                this.Log.LogMessage
                    (
                        MessageImportance.Low,
                        $"Updated project metadata for project: {project.GetMetadata("ProjectName")}"
                    );
            }

            // set the projects output
            this.Projects = this.projects.ToArray();

            // assume its always true
            return true;
        }

        private void SetMetadata(ITaskItem project)
        {
            // get the full path of the project file
            var path = project.GetMetadata("FullPath");

            // get the directory name from the path
            var directory = Path.GetDirectoryName(path);
            var parent = Path.GetDirectoryName(directory);
            var group = Path.GetFileName(directory);

            // determine if this is an msbuild project
            var msbuild = path.EndsWith("proj");

            // get the project name
            var projectName = msbuild ? Path.GetFileNameWithoutExtension(path) : group;

            // determine if the group is a well-known folder path
            if (!WellKnownFolders.Contains(group, StringComparer.OrdinalIgnoreCase))
            {
                // use the parent of the group folder, which means multiple projects are contained within the folder
                group = Path.GetFileName(parent);
            }

            // set the group to lower
            group = group.ToLower();

            // set the project directory path
            project.SetMetadata("ProjectDir", directory + Path.DirectorySeparatorChar);

            // set the project group
            project.SetMetadata("ProjectGroup", group);

            // set the name of the project based on the name of the csproj
            project.SetMetadata("ProjectName", projectName);

            // determine if the project is not an msbuild project
            if (!msbuild)
            {
                // add the project to the output
                this.projects.Add(project);

                // move on immediately
                return;
            }

            // set the description
            project.SetMetadata("Description", Path.Combine(group, projectName));

            // get the root output path
            var root = Path.Combine(directory, "obj", "docker", "publish");

            // set the shared sources directory
            project.SetMetadata("SharedSourcesDir", Path.Combine(parent, "shared") + Path.DirectorySeparatorChar);

            // set the condo assembly info path
            project.SetMetadata("CondoAssemblyInfo", Path.Combine(directory, "Properties", "Condo.AssemblyInfo.cs"));

            // create the targets
            var targets = new List<string>();

            // determine if restore is enabled
            if (this.Restore)
            {
                // set the is restorable bit
                project.SetMetadata("IsRestorable", true.ToString());
            }

            // determine if build is enabled
            if (this.Build)
            {
                // set the is buildable bit
                project.SetMetadata("IsBuildable", true.ToString());
            }

            // create the xml variable
            var xml = default(XDocument);

            try
            {
                // parse the file
                xml = XDocument.Load(path);
            }
            catch (XmlException xmlEx)
            {
                // log the eception as an error
                this.Log.LogErrorFromException(xmlEx, showStackTrace: true, showDetail: true, file: path);

                // move on immediately
                return;
            }

            // get the output type (default to library)
            var output = xml.Descendants("OutputType").FirstOrDefault()?.Value.ToLower() ?? "library";

            // set the output type
            project.SetMetadata("OutputType", output);

            // set publish and pack to false
            project.SetMetadata("IsBuildable", this.Build.ToString());
            project.SetMetadata("IsRestorable", this.Restore.ToString());
            project.SetMetadata("IsPublishable", false.ToString());
            project.SetMetadata("IsPackable", false.ToString());
            project.SetMetadata("IsTestable", false.ToString());
            project.SetMetadata("SelfContained", false.ToString());

            // get the target framework node
            var frameworks = xml.Descendants("TargetFramework").Union(xml.Descendants("TargetFrameworks"))
                .Distinct()
                .SelectMany(node => node.Value.Split(';'))
                .OrderByDescending(name => name);

            // determine if the frameworks node did not exist
            if (frameworks == null || !frameworks.Any())
            {
                // add the project to the output
                this.projects.Add(project);

                // move on immediately
                return;
            }

            // get the highest netcore tfm
            var tfm = frameworks
                .FirstOrDefault(name => name.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase));

            // determine if the project is a library
            var library = output.Equals("library", StringComparison.OrdinalIgnoreCase);

            // determine if the project is in the source group
            if (string.Equals(group, "src", StringComparison.OrdinalIgnoreCase))
            {
                // set the publish to true
                if (this.Publish)
                {
                    project.SetMetadata("IsPublishable", (!library).ToString());
                    project.SetMetadata("IsPublishable", (tfm != null).ToString());
                    project.SetProperty("IsPublishable", xml);
                }

                // set the pack to true
                if (this.Pack)
                {
                    project.SetMetadata("IsPackable", library.ToString());
                    project.SetProperty("IsPackable", xml);
                }
            }

            // determine if this is a test project
            if (this.Test && string.Equals(group, nameof(this.Test), StringComparison.OrdinalIgnoreCase))
            {
                // set the default publish and pack
                project.SetMetadata("IsTestable", true.ToString());
                project.SetProperty("IsTestable", xml);
            }

            // get the target framework node
            var rids = xml.Descendants("RuntimeIdentifier").Union(xml.Descendants("RuntimeIdentifiers"))
                .Distinct()
                .SelectMany(node => node.Value.Split(';'))
                .OrderByDescending(name => name);

            // iterate over each framework
            foreach (var framework in frameworks)
            {
                // create a new project
                var any = new TaskItem(project.ItemSpec);

                // copy metadata from the project
                project.CopyMetadataTo(any);

                // update the description
                any.SetMetadata("Description", Path.Combine(group, projectName, framework));

                // add the project to the project collection
                this.projects.Add(any);

                // set the target framework
                any.SetMetadata("TargetFramework", framework);
                any.SetMetadata("RuntimeIdentifier", string.Empty);
                any.SetMetadata("OutputPath", Path.Combine(root, framework, "dotnet") + Path.DirectorySeparatorChar);

                // iterate over each runtime identifier
                foreach (var rid in rids)
                {
                    // create the publish path
                    var publish = Path.Combine(root, framework, rid);

                    // create the publish directory
                    Directory.CreateDirectory(publish);

                    // create a self contained project
                    var contained = new TaskItem(project.ItemSpec);

                    // copy the metadata
                    any.CopyMetadataTo(contained);

                    // update the description
                    any.SetMetadata("Description", Path.Combine(group, projectName, framework, rid));

                    // add the project to the project collection
                    this.projects.Add(contained);

                    // update the target framework and runtime identifier
                    contained.SetMetadata("RuntimeIdentifier", rid);
                    contained.SetMetadata("IsRestorable", false.ToString());
                    contained.SetMetadata("IsBuildable", false.ToString());
                    contained.SetMetadata("SelfContained", true.ToString());

                    contained.SetMetadata("OutputPath", Path.Combine(root, framework, rid) + Path.DirectorySeparatorChar);
                }
            }
        }
        #endregion
    }
}
