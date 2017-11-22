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

    using Microsoft.Build.Construction;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Exceptions;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task used to set additional project metadata for .NET CoreCLR projects using the
    /// modern MSBuild format.
    /// </summary>
    public class GetProjectMetadata : Task
    {
        #region Private Fields
        /// <summary>
        /// The list of well-known folders used for project layouts.
        /// </summary>
        public static readonly string[] WellKnownFolders = { "src", "test", "docs", "samples" };

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
        /// Gets or sets the product name to use when only a single project is present.
        /// </summary>
        [Required]
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

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

        private void SetMetadata(ITaskItem item)
        {
            // get the full path of the project file
            var path = item.GetMetadata("FullPath");

            // get the file name
            var file = item.GetMetadata("FileName");

            // get the directory name from the path
            var directory = Path.GetDirectoryName(path);
            var parent = Path.GetDirectoryName(directory);
            var group = Path.GetFileName(directory);

            // get the product name
            var projectName = this.Product;

            // determine if the project is rooted
            var rooted = string.Equals
            (
                Path.GetFullPath(directory + Path.DirectorySeparatorChar),
                Path.GetFullPath(this.RepositoryRoot + Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase
            );

            // determine if we are not rooted and the group is a well-known path
            if (!rooted && !WellKnownFolders.Contains(group, StringComparer.OrdinalIgnoreCase))
            {
                // set the project name to the group
                projectName = group;

                // use the parent of the group folder, which means multiple projects are contained within the folder
                group = Path.GetFileName(parent);
            }

            // determine if this is an msbuild project
            var msbuild = path.EndsWith("proj");

            // if the project is an msbuild project
            if (msbuild)
            {
                // set the project name to the project name
                projectName = Path.GetFileNameWithoutExtension(path);
            }

            // set the group to lower
            projectName = projectName.ToLower();

            // set the project directory path
            item.SetMetadata("ProjectDir", directory + Path.DirectorySeparatorChar);

            // set the project group
            item.SetMetadata("ProjectGroup", group);

            // set the name of the project based on the name of the csproj
            item.SetMetadata("ProjectName", projectName);

            // determine if the project is not an msbuild project
            if (!msbuild)
            {
                // add the project to the output
                this.projects.Add(item);

                // move on immediately
                return;
            }

            // set the description
            item.SetMetadata("Description", Path.Combine(group, projectName));

            // get the root output path
            var root = Path.Combine(directory, "obj", "docker", "publish");

            // set the shared sources directory
            item.SetMetadata("SharedSourcesDir", Path.Combine(parent, "shared") + Path.DirectorySeparatorChar);

            // set the condo assembly info path
            item.SetMetadata("CondoAssemblyInfo", Path.Combine(directory, "Properties", "Condo.AssemblyInfo.cs"));

            // create the targets
            var targets = new List<string>();

            // set the restore and build properties
            item.SetMetadata("IsRestorable", this.Restore.ToString());
            item.SetMetadata("IsBuildable", this.Build.ToString());

            // create a project collection
            var collection = new ProjectCollection();

            // define a variable to retain the project
            var project = default(Project);

            try
            {
                // attempt to load the project
                project = new Project(path, null, null, collection);
            }
            catch (InvalidProjectFileException buildEx)
            {
                // log the eception as an error
                this.Log.LogErrorFromException(buildEx, showStackTrace: true, showDetail: true, file: path);

                // move on immediately
                return;
            }

            // capture all of the project properties
            var properties = project.AllEvaluatedProperties;

            // get the output type
            var output = properties.GetEvaluatedValue("OutputType") ?? "library";

            // set the output type
            item.SetMetadata("OutputType", output);

            // set publish and pack to false
            item.SetMetadata("IsPublishable", false.ToString());
            item.SetMetadata("IsPackable", false.ToString());
            item.SetMetadata("IsTestable", false.ToString());
            item.SetMetadata("SelfContained", false.ToString());

            // initialize the frameworks
            var frameworks = new List<string>();

            // attempt to get the vanilla target framework moniker
            if (properties.TryGetEvaluatedValue("TargetFramework", out string tfm))
            {
                // add the tfm to the frameworks
                frameworks.Add(tfm);
            }

            // attempt to get the target framework list
            if (properties.TryGetEvaluatedValue("TargetFrameworks", out string tfms))
            {
                // add the tfms to the frameworks
                frameworks.AddRange(tfms.Split(';'));
            }

            // determine if at least one framework exists
            if (!frameworks.Any())
            {
                // add the project to the output
                this.projects.Add(item);

                // move on immediately
                return;
            }

            // order the frameworks
            frameworks = frameworks.Distinct().OrderByDescending(name => name).ToList();

            // determine if the project is publishable
            var publishable = frameworks
                .Any(name => name.StartsWith("netcore", StringComparison.OrdinalIgnoreCase));

            // determine if the project is a library
            var library = output.Equals("library", StringComparison.OrdinalIgnoreCase);

            // determine if the project is in the source group
            if (string.Equals(group, "src", StringComparison.OrdinalIgnoreCase))
            {
                // set the publish to true
                if (this.Publish)
                {
                    item.SetMetadata("IsPublishable", publishable.ToString());
                    item.SetProperty("IsPublishable", properties);
                }

                // set the pack to true
                if (this.Pack)
                {
                    item.SetMetadata("IsPackable", library.ToString());
                    item.SetProperty("IsPackable", properties);
                }
            }

            // determine if this is a test project
            if (this.Test && string.Equals(group, nameof(this.Test), StringComparison.OrdinalIgnoreCase))
            {
                // set the default publish and pack
                item.SetMetadata("IsTestable", true.ToString());
                item.SetProperty("IsTestable", properties);
            }

            // create a runtime identifiers list
            var runtimeIdentifiers = new List<string>();

            // try to get the vanilla runtime identifier
            if (properties.TryGetEvaluatedValue("RuntimeIdentifier", out string rid))
            {
                // add the rid to the runtime identifiers
                runtimeIdentifiers.Add(rid);
            }

            // try to get the runtime identifiers list
            if (properties.TryGetEvaluatedValue("RuntimeIdentifiers", out string rids))
            {
                // add the range of rids
                runtimeIdentifiers.AddRange(rids.Split(';'));
            }

            // order the runtime identifiers
            runtimeIdentifiers = runtimeIdentifiers.Distinct().OrderByDescending(name => name).ToList();

            // iterate over each framework
            foreach (var framework in frameworks)
            {
                // create a new project
                var any = new TaskItem(item.ItemSpec);

                // copy metadata from the project
                item.CopyMetadataTo(any);

                // update the description
                any.SetMetadata("Description", Path.Combine(group, projectName, framework));

                // add the project to the project collection
                this.projects.Add(any);

                // set the target framework
                any.SetMetadata("TargetFramework", framework);
                any.SetMetadata("RuntimeIdentifier", string.Empty);
                any.SetMetadata("OutputPath", Path.Combine(root, framework, "dotnet") + Path.DirectorySeparatorChar);
                any.SetMetadata("TestLogFileName", string.Join('.', file, framework, "dotnet"));

                // iterate over each runtime identifier
                foreach (var runtimeIdentifier in runtimeIdentifiers)
                {
                    // create the publish path
                    var publish = Path.Combine(root, framework, runtimeIdentifier);

                    // create the publish directory
                    Directory.CreateDirectory(publish);

                    // create a self contained project
                    var contained = new TaskItem(item.ItemSpec);

                    // copy the metadata
                    any.CopyMetadataTo(contained);

                    // update the description
                    any.SetMetadata("Description", Path.Combine(group, projectName, framework, runtimeIdentifier));

                    // add the project to the project collection
                    this.projects.Add(contained);

                    // update the target framework and runtime identifier
                    contained.SetMetadata("RuntimeIdentifier", runtimeIdentifier);
                    contained.SetMetadata("IsRestorable", false.ToString());
                    contained.SetMetadata("IsBuildable", false.ToString());
                    contained.SetMetadata("SelfContained", true.ToString());

                    // set the output path
                    contained.SetMetadata("OutputPath", Path.Combine(root, framework, runtimeIdentifier) + Path.DirectorySeparatorChar);
                    contained.SetMetadata("TestLogFileName", string.Join('.', file, framework, runtimeIdentifier));
                }
            }
        }
        #endregion
    }
}
