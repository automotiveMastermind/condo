// <copyright file="GetProjectsForSolution.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AM.Condo.Diagnostics;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build (MSBuild) task used to discover projects from a solution file.
    /// </summary>
    public class GetProjectsForSolution : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the solutions for which to discover projects.
        /// </summary>
        [Required]
        public ITaskItem[] Solutions { get; set; }

        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets the projects associated with the solutions.
        /// </summary>
        [Output]
        public ITaskItem[] Projects { get; private set; }
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Execute()
        {
            // determine if any solutions are available
            if (this.Solutions.Length == 0)
            {
                // move on immediately
                return true;
            }

            // gather invalid solutions
            var invalid = this.Solutions
                .Where(solution => !solution.GetMetadata("Extension").Equals(".sln", StringComparison.OrdinalIgnoreCase));

            // determine if any invalid solution files exist
            if (invalid.Any())
            {
                // collect the item specifications
                var paths = invalid.Select(item => item.ItemSpec);

                // log the error
                this.Log.LogError($"The following items are not valid solution files:{Environment.NewLine}{string.Join(Environment.NewLine, paths)}");

                // move on immediately
                return false;
            }

            // capture the solution paths
            var solutions = this.Solutions.Select(solution => solution.GetMetadata("FullPath"));

            // create the project list
            var projects = new List<TaskItem>();

            // create a new process invoker
            var process = new ProcessInvoker(this.RepositoryRoot, "dotnet", "sln", new CondoMSBuildLogger(this.Log));

            // iterate over each solution
            foreach (var solution in solutions)
            {
                // collect the output
                var output = process.Execute($@"""{solution}"" list", throwOnError: true).Output;

                // collect items
                var items = output.Where(line => line.EndsWith("proj", StringComparison.OrdinalIgnoreCase))
                    .Select(line => new TaskItem(line));

                // add the items to the projects list
                projects.AddRange(items);
            }

            // set the projects array
            this.Projects = projects.ToArray();

            // assume success
            return true;
        }
        #endregion
    }
}
