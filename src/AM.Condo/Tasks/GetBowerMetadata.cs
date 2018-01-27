// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetBowerMetadata.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using AM.Condo.Resources;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a Microsoft Build task that retrieves project metadata from a bower project file.
    /// </summary>
    public class GetBowerMetadata : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the project for which to retrieve metadata.
        /// </summary>
        [Required]
        [Output]
        public ITaskItem[] Projects { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetBowerMetadata"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // create a list to retain the results
            var projects = new List<ITaskItem>();

            // iterate over each project file
            foreach (var project in this.Projects)
            {
                try
                {
                    // set the metadata for the project
                    this.SetMetadata(project);

                    // log a message
                    this.Log.LogMessage
                        (
                            MessageImportance.Low,
                            $"Updated project metadata for project: {project.GetMetadata("ProjectName")}"
                        );

                    // append the project to the list
                    projects.Add(project);
                }
                catch (Exception netEx)
                {
                    // log the exception as a warning
                    this.Log.LogWarningFromException(netEx);
                }
            }

            // update the output
            this.Projects = projects.ToArray();

            // move on immediately
            return true;
        }

        private void SetMetadata(ITaskItem project)
        {
            // get the full path of the project
            var path = project.GetMetadata("FullPath");

            // set the project directory path
            project.SetMetadata("ProjectDir", Path.GetDirectoryName(path));

            // load the json for the path
            var json = File.ReadAllText(path);

            // deserialize the object the json
            var bower = JsonConvert.DeserializeObject<BowerProject>(json);

            // set the metadata
            project.SetMetadata("ProjectName", bower.Name);
            project.SetMetadata("ProjectVersion", bower.Version);
            project.SetMetadata("ProjectLicense", bower.License);
        }
        #endregion
    }
}
