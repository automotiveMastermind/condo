namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.IO;

    using static System.FormattableString;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Tasks;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that sets a tag for the current commit with git.
    /// </summary>
    public class SetGitTag : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the tag that should be created with git.
        /// </summary>
        [Required]
        [Output]
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the remote that should be used to push the tag.
        /// </summary>
        /// <returns></returns>
        [Output]
        public string Remote { get; set; } = "origin";

        /// <summary>
        /// Gets or sets the annotation for the tag. If no annotation is specified, the annotation will be set to the
        /// tag value, which includes additional data about who created the tag and when.
        /// </summary>
        [Output]
        public string Annotation { get; set; }

        /// <summary>
        /// Gets the version of the client used to access the repository.
        /// </summary>
        [Output]
        public string ClientVersion { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetRepositoryInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // determine if the root is specified
            if (string.IsNullOrEmpty(this.RepositoryRoot))
            {
                // log the error
                this.Log.LogError("The repository root must be specified.");

                // move on immediately
                return false;
            }

            if (!Directory.Exists(this.RepositoryRoot))
            {
                // log the error
                this.Log.LogError($"The repository root {this.RepositoryRoot} does not exist or could not be found.");

                // move on immediately
                return false;
            }

            // determine if the tag is set
            if (string.IsNullOrEmpty(this.Tag))
            {
                // log the error
                this.Log.LogError("The tag must be set to a non-empty value.");

                // move on immediately
                return false;
            }

            // determine if the remote is set
            if (string.IsNullOrEmpty(this.Remote))
            {
                // log the error
                this.Log.LogError("The remote must be specified.");
            }

            // create an exec task for retrieving the version
            var exec = this.CreateExecTask("--version");

            // execute the command and ensure that the output exists
            if (!exec.Execute())
            {
                // log a warning
                Log.LogWarning("The git command line tool is not available on the current path.");

                // move on immediately
                return true;
            }

            // determine if the output did not exist
            if (exec.ConsoleOutput.Length == 0)
            {
                // log a warning
                Log.LogWarning("The version of the git command line tool is unavailable.");

                // move on immediately
                return true;
            }

            // set the client version
            this.ClientVersion = exec.ConsoleOutput[0].ItemSpec;

            // determine if the annotation is not set
            if (string.IsNullOrEmpty(this.Annotation))
            {
                // set the annotation to the tag
                this.Annotation = this.Tag;
            }

            // replace whitespace characters in the tag
            this.Tag = this.Tag.Replace(' ', '-').Replace(Environment.NewLine, "-");

            // create the task used to set the git tag
            exec = this.CreateExecTask($@"tag -a {this.Tag} -m ""{this.Annotation}""");

            // execute the task
            if (!exec.Execute())
            {
                // log an error indicating that the tag could not be created
                Log.LogWarning($"Failed to create the git tag {this.Tag}");

                // move on immediately
                return true;
            }

            // create the task to push the tag to the remote
            exec = this.CreateExecTask($"push {this.Remote} --tags");

            // execute the task
            if (!exec.Execute())
            {
                // log an error indicating that the tag could not be created
                Log.LogWarning($"Failed to push the git tag {this.Tag} to the remote {this.Remote}");
            }

            // execute the push
            return true;
        }

        /// <summary>
        /// Creates a Microsoft Build execution task used to execute a git command.
        /// </summary>
        /// <param name="command">
        /// The git command that should be executed.
        /// </param>
        /// <returns>
        /// The execution task that can be used to execute the git command.
        /// </returns>
        private Exec CreateExecTask(string command)
        {
            // create a new exec
            return new QuietExec
            {
                Command = Invariant($"git {command}"),
                WorkingDirectory = this.RepositoryRoot,
                BuildEngine = this.BuildEngine,
                ConsoleToMSBuild = true,
                EchoOff = true
            };
        }
        #endregion
    }
}