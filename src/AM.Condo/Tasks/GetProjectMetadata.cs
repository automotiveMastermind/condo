// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProjectMetadata.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task used to set additional project metadata for .NET CoreCLR projects using the
    /// project.json format.
    /// </summary>
    public class GetProjectMetadata : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the list of projects for which to set additional metadata.
        /// </summary>
        [Required]
        [Output]
        public ITaskItem[] Projects { get; set; }
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

            // assume its always true
            return true;
        }

        private static void SetMSBuildMetadata(ITaskItem project, string path)
        {
            // set the dotnet build type
            project.SetMetadata("DotNetType", "MSBuild");

            // parse the file
            var xml = XDocument.Load(path);

            // get the output type (default to library)
            var output = xml.Descendants("OutputType").FirstOrDefault()?.Value ?? "library";

            // determine if the output type is available
            if (!string.IsNullOrEmpty(output))
            {
                // set the output type
                project.SetMetadata("OutputType", output.ToLower());
            }

            // get the target framework node
            var frameworks = xml.Descendants("TargetFramework");

            // determine if the frameworks node did not exist
            if (frameworks == null || !frameworks.Any())
            {
                // move on immediately
                return;
            }

            // get the name properties ordered by name
            var names = frameworks.SelectMany(node => node.Value.Split(';'))
                .OrderByDescending(name => name);

            // get the highest netcore tfm
            var tfm = names.FirstOrDefault(name => name.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase));

            // set the target frameworks property
            project.SetMetadata("TargetFrameworks", string.Join(";", names));
            project.SetMetadata("NetCoreFramework", tfm);
        }

        private void SetMetadata(ITaskItem project)
        {
            // get the full path of the project file
            var path = project.GetMetadata("FullPath");

            // get the directory name from the path
            var directory = Path.GetDirectoryName(path);
            var group = Path.GetFileName(Path.GetDirectoryName(directory));

            // set the project directory path
            project.SetMetadata("ProjectDir", directory + Path.DirectorySeparatorChar);

            // set the project group
            project.SetMetadata("ProjectGroup", group);

            // set the name of the project (using the directory name by convention)
            project.SetMetadata("ProjectName", Path.GetFileName(directory));

            // set the shared sources directory
            project.SetMetadata("SharedSourcesDir", Path.Combine(directory, "shared") + Path.DirectorySeparatorChar);

            // set the condo assembly info path
            project.SetMetadata("CondoAssemblyInfo", Path.Combine(directory, "Properties", "Condo.AssemblyInfo.cs"));

            // set msbuild metadata
            SetMSBuildMetadata(project, path);
        }
        #endregion
    }
}
