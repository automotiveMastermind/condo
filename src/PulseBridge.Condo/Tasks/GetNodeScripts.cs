namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.IO;

    using static System.FormattableString;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Microsoft.Build.Tasks;

    /// <summary>
    /// Represents a Microsoft Build task that gets a list of available node scripts.
    /// </summary>
    public class GetNodeScripts : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the project for which to retrieve the available node scripts.
        /// </summary>
        [Required]
        public ITaskItem Project { get; set; }

        /// <summary>
        /// Gets the scripts that are associated with the project.
        /// </summary>
        [Output]
        public ITaskItem[] Scripts { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not a script for condo is specified within the node scripts.
        /// </summary>
        [Output]
        public bool HasCondo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not a script for continuous integration is specified within the node
        /// scripts.
        /// </summary>
        [Output]
        public bool HasCI { get; private set; }

        /// <summary>
        /// Gets the version of the NPM client that is currently available on the command line.
        /// </summary>
        [Output]
        public string ClientVersion { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetNodeScripts"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // determine if the project is specified
            if (this.Project == null)
            {
                // log the error
                this.Log.LogError("no project was specified.");

                // return false
                return false;
            }

            // closure of the project
            var project = this.Project;

            // get the full path of the project
            var path = project.GetMetadata("FullPath");

            // get the file name of the project
            var name = Path.GetFileName(path);

            // determine if the file is a node project
            if (!string.Equals(name, "package.json", StringComparison.OrdinalIgnoreCase))
            {
                // log a warning
                this.Log.LogWarning(Invariant($"the project at {path} is not a NodeJS project and has been skipped"));

                // move on immediately
                return true;
            }

            // get the parent directory of the project
            var working = Path.GetDirectoryName(path);

            // create the version task
            var exec = this.CreateExecTask("--version", working);

            // execute the command and ensure that the output exists
            if (!exec.Execute())
            {
                // log a warning
                Log.LogWarning("The npm command line tool is not available on the current path.");

                // move on immediately
                return true;
            }

            // determine if the output did not exist
            if (exec.ConsoleOutput.Length == 0)
            {
                // log a warning
                Log.LogWarning("The version of the npm command line tool is unavailable.");

                // move on immediately
                return true;
            }

            // set the client version
            this.ClientVersion = exec.ConsoleOutput[0].ItemSpec;

            // create the npm task
            exec = this.CreateExecTask("run --porcelain", working);

            // execute the command and enure that it was successful
            if (!exec.Execute())
            {
                // log an npm warning
                Log.LogWarning("Could not retrieve a list of available npm scripts.");

                // move on immediately
                return true;
            }

            // iterate over each output
            foreach (var output in exec.ConsoleOutput)
            {
                // get the index of the colon
                var colon = output.ItemSpec.IndexOf(':');

                // determine if a colon was found
                if (colon == -1)
                {
                    // write a warning
                    this.Log.LogWarning(Invariant($"output from NPM for script {output.ItemSpec} in project {name} was malformed; skipping"));

                    // continue
                    continue;
                }

                // get the name of the script
                var script = output.ItemSpec.Substring(0, colon);

                // determine if the script is a condo script
                if (script.Equals("condo", StringComparison.OrdinalIgnoreCase))
                {
                    // set the has condo to true
                    this.HasCondo = true;
                }

                // determine if the script is a ci script
                if (script.Equals("ci", StringComparison.OrdinalIgnoreCase))
                {
                    // set the has ci to true
                    this.HasCI = true;
                }

                // get the body of the script
                var body = output.ItemSpec.Substring(colon + 1);

                // set the script name metadata
                output.SetMetadata("ScriptName", script);
                output.SetMetadata("ScriptBody", body);
            }

            // set the scripts to the console output
            this.Scripts = exec.ConsoleOutput;

            // we were successful
            return true;
        }

        /// <summary>
        /// Creates a Microsoft Build execution task used to execute a git command.
        /// </summary>
        /// <param name="command">
        /// The git command that should be executed.
        /// </param>
        /// <param name="root">
        /// The root path in which to execute the git command.
        /// </param>
        /// <returns>
        /// The execution task that can be used to execute the git command.
        /// </returns>
        private Exec CreateExecTask(string command, string root)
        {
            // create a new exec
            return new QuietExec
            {
                Command = Invariant($"npm {command}"),
                WorkingDirectory = root,
                BuildEngine = this.BuildEngine,
                ConsoleToMSBuild = true,
                EchoOff = true
            };
        }
        #endregion
    }
}