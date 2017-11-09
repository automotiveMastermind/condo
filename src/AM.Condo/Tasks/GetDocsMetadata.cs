// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetDocsMetadata.cs" company="automotiveMastermind and contributors">
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
    using NuGet.Versioning;

    /// <summary>
    /// Represents a Microsoft Build task used to set additional project metadata for documentation generation projects.
    /// </summary>
    public class GetDocsMetadata : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the list of documentation generation projects for which to set additional metadata.
        /// </summary>
        [Required]
        [Output]
        public ITaskItem[] Projects { get; set; }

        /// <summary>
        /// Gets or sets the root path for documentation generation.
        /// </summary>
        [Required]
        public string DocsRoot { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetDocsMetadata"/> task.
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
                        $"Updated metadata for documentation generation project: {project.GetMetadata("ProjectName")}"
                    );
            }

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
            var group = "docs";

            // set the group
            project.SetMetadata("Group", group);

            // get the directory name as the project name
            var projectName = Path.GetFileName(directory);

            // determine if the directory is the group name
            if (string.Equals(projectName, group, StringComparison.OrdinalIgnoreCase))
            {
                // get the file name of the parent as the project name
                projectName = Path.GetFileName(parent);
            }

            // lower the project name
            projectName = projectName.ToLower();

            // set the project name
            project.SetMetadata("ProjectName", projectName);
            project.SetMetadata("ProjectDir", directory + Path.DirectorySeparatorChar);

            // determine if the project is rooted
            var rooted = string.Equals
            (
                Path.GetFullPath(directory + Path.DirectorySeparatorChar),
                Path.GetFullPath(this.DocsRoot + Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase
            );

            // set the rooted flag
            project.SetMetadata("IsRootDocs", rooted.ToString());

            // set the output path
            project.SetMetadata("OutputPath", Path.Combine(directory, "bin"));
        }
        #endregion
    }
}
