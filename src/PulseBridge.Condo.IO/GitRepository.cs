namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using static System.FormattableString;

    using PulseBridge.Condo.Diagnostics;

    /// <summary>
    /// Represents a reference to a git repository that can be manipulated programmatically.
    /// </summary>
    public class GitRepository : IGitRepository
    {
        #region Private Fields
        private readonly IPathManager path;

        private readonly string version;

        private bool disposed;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        /// <param name="path">
        /// The path manager that is responsible for managing the path.
        /// </param>
        public GitRepository(IPathManager path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), Invariant($"The {nameof(path)} cannot be null."));
            }

            this.path = path;

            try
            {
                var version = this.Execute("--version");

                this.version = version.Output.First();
            }
            catch (Exception netEx)
            {
                throw new InvalidOperationException
                    (Invariant($"A git client was not found on the current path ({path.FullPath})."), netEx);
            }
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public string CurrentBranch
        {
            get
            {
                // get the output for the current branch
                var result = this.Execute("symbolic-ref HEAD");

                // determine if we were not successful
                if (!result.Success)
                {
                    // return null
                    return null;
                }

                var branch = result.Output.FirstOrDefault();

                // return the current branch
                return branch.StartsWith("refs/heads/")
                    ? branch.Substring(11)
                    : branch;
            }
        }

        /// <inheritdoc/>
        public string ClientVersion => this.version;

        /// <inheritdoc/>
        public string RepositoryPath => this.path.FullPath;

        /// <inheritdoc/>
        public string Username
        {
            get
            {
                var result = this.Execute("config --get user.name");

                // return the username
                return result.Success ? result.Output.FirstOrDefault() : null;
            }

            set
            {
                this.Execute($@"config user.name ""{value}""");
            }
        }

        /// <inheritdoc/>
        public string Email
        {
            get
            {
                var result = this.Execute("config --get user.email");

                // return the email
                return result.Success ? result.Output.FirstOrDefault() : null;
            }

            set
            {
                this.Execute($@"config user.email ""{value}""");
            }
        }

        /// <inheritdoc/>
        public string OriginUri
        {
            get
            {
                // get the result
                var result = this.Execute("ls-remote --get-url origin");

                // return the origin uri
                return result.Success ? result.Output.FirstOrDefault() : null;
            }
        }

        /// <inheritdoc/>
        public string LatestCommit
        {
            get
            {
                var result = this.Execute("rev-parse HEAD");

                return result.Success ? result.Output.FirstOrDefault() : null;
            }
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public IGitRepositoryInitialized Initialize()
        {
            // execute the init command
            this.Execute("init");

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryBare Bare()
        {
            this.Execute("init --bare");

            return this;
        }

        public IGitRepositoryInitialized Clone(string uri)
        {
            // create the cmd to clone the repository
            var cmd = $"clone {uri} .";

            // execute the cmd
            this.Execute(cmd);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Save(string relativePath, string contents)
        {
            // save the contents using the path manager
            this.path.Save(relativePath, contents);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Add(string spec)
        {
            this.Execute($@"add ""{spec}""");

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Branch(string name, string source)
        {
            var cmd = $"checkout -b {name}";

            if (!string.IsNullOrEmpty(source))
            {
                cmd += $" {source}";
            }

            this.Execute(cmd);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Checkout(string name)
        {
            this.Execute($"checkout {name}");

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Commit(string message)
        {
            this.Execute($@"commit --allow-empty -m ""{message}""");

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Tag(string name)
        {
            this.Execute($"tag {name}");

            return this;
        }

        /// <inheritdoc/>
        public IProcessOutput Execute(string command)
        {
            var start = this.CreateProcessInfo(command);
            var process = new Process
            {
                StartInfo = start,
                EnableRaisingEvents = true
            };

            // create queues for data
            var errorQueue = new Queue<string>();
            var outputQueue = new Queue<string>();
            var exitCode = -1;

            // setup event handlers to handle queue processing
            process.ErrorDataReceived += (sender, args) => errorQueue.Enqueue(args.Data);
            process.OutputDataReceived += (sender, args) => outputQueue.Enqueue(args.Data);

            try
            {
                // start the process
                process.Start();

                // start listening for events
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                // do not write to standard input
                process.StandardInput.Dispose();

                // wait for exit
                process.WaitForExit();

                // spinwait to ensure exit
                SpinWait.SpinUntil(() => process.HasExited);

                // determine if the process did not exit successfully
                if (process.ExitCode != 0)
                {
                    // enqueue the error
                    errorQueue.Enqueue
                        (Invariant($"Execution of {command} failed in path {this.RepositoryPath}. Exit Code: {process.ExitCode}"));
                }
            }
            catch (Exception netEx)
            {
                errorQueue.Enqueue
                    (Invariant($"Execution of {command} failed in path {this.RepositoryPath}. Error: {netEx.Message}"));
            }
            finally
            {
                exitCode = process.ExitCode;
                process.Dispose();
            }

            // return process output
            return new ProcessOutput(outputQueue, errorQueue, exitCode);
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Condo(string root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root), $"The {nameof(root)} parameter cannot be null.");
            }

            if (string.IsNullOrEmpty(root))
            {
                throw new ArgumentException($"The {nameof(root)} parameter cannot be empty.", nameof(root));
            }

            if (!Directory.Exists(root))
            {
                throw new ArgumentException($"The {nameof(root)} path does not exist.", nameof(root));
            }

            var template = Path.Combine(root, "template");

            if (!Directory.Exists(template))
            {
                throw new ArgumentException
                    ($"The {nameof(root)} path does not contain the condo template.", nameof(root));
            }

            foreach (var source in Directory.GetFiles(template))
            {
                var destination = Path.Combine(this.RepositoryPath, Path.GetFileName(source));

                File.Copy(source, destination);
            }

            File.Copy(Path.Combine(root, "condo.build"), Path.Combine(this.RepositoryPath, "condo.build"));

            return this;
        }

        /// <inheritdoc />
        public IGitLog Log(string from, string to, IGitLogOptions options, IGitLogParser parser)
        {
            const string Split = "------------------------ >8 ------------------------";

            // create the range
            var range = string.IsNullOrEmpty(from) ? to : $"{from}..{to}";

            // create the command used to get the history of commits
            var exec = this.Execute($@"log {range} --format=""%H%n%h%n%s%n%b%n{Split}%n""");

            // determine if we were successful
            if (!exec.Success)
            {
                // move on immediately
                return null;
            }

            // parse the output
            return parser.Parse(exec.Output, options);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not dispose was called manually.
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(GitRepository));
            }

            this.disposed = true;

            if (!disposing)
            {
                return;
            }

            this.path.Dispose();
        }

        private ProcessStartInfo CreateProcessInfo(string command)
        {
            return new ProcessStartInfo("git", command)
            {
                WorkingDirectory = this.path.FullPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };
        }
        #endregion
    }
}