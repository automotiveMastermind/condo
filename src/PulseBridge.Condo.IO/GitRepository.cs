namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using static System.FormattableString;

    using PulseBridge.Condo.Diagnostics;

    /// <summary>
    /// Represents a reference to a git repository that can be manipulated programmatically.
    /// </summary>
    public class GitRepository : IGitRepository
    {
        #region Private Fields
        private const string Split = "------------------------ >8< ------------------------";

        private const string Format = "%H%n%h%n%cI%n%D%n%B%n" + Split;

        private readonly IPathManager path;

        private readonly ILogger logger;

        private readonly Version version;

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
            : this(path, new NoOpLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        /// <param name="path">
        /// The path manager that is responsible for managing the path.
        /// </param>
        /// <param name="log">
        /// The logger that is responsible for logging.
        /// </param>
        public GitRepository(IPathManager path, ILogger log)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            this.path = path;
            this.logger = log;

            try
            {
                // get the version outut
                var output = this.Execute("--version");

                // capture the version string
                var values = output.Output.FirstOrDefault()?.Split(' ');

                // iterate over each value
                foreach (var value in values)
                {
                    // attempt to parse the version
                    if (Version.TryParse(value, out this.version))
                    {
                        // break from the iteration
                        break;
                    }
                }
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
                    // log the output as a warning
                    this.logger.LogWarning(result.Error);

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
        public string ClientVersion => this.version?.ToString();

        /// <inheritdoc/>
        public string RepositoryPath => this.path?.FullPath;

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
                var output = this.Execute($@"config user.name ""{value}""");

                if (!output.Success)
                {
                    throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
                }
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
                var output = this.Execute($@"config user.email ""{value}""");

                if (!output.Success)
                {
                    throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
                }
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

        /// <inheritdoc/>
        public IEnumerable<string> Tags
        {
            get
            {
                var cmd = this.version?.Major > 1 ? "tag --sort version:refname" : "tag";

                var result = this.Execute(cmd);

                return result.Success ? result.Output : new List<string>();
            }
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public IGitRepositoryInitialized Initialize()
        {
            // execute the init command
            var output = this.Execute("init");

            if (!output.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
            }

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Push(string remote, bool tags)
        {
            var cmd = "push";

            if (tags)
            {
                cmd += " --tags";
            }

            if (!string.IsNullOrEmpty(remote))
            {
                cmd += $" --set-upstream {remote}";
            }

            var output = this.Execute(cmd);

            if (!output.Success)
            {
                this.logger.LogWarning(output.Error);
            }

            this.logger.LogMessage(output.Output);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Pull(bool all)
        {
            var cmd = "pull";

            if (all)
            {
                cmd += " --all";
            }

            var output = this.Execute(cmd);

            if (!output.Success)
            {
                this.logger.LogWarning(output.Error);
            }

            this.logger.LogMessage(output.Output);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryBare Bare()
        {
            var output = this.Execute("init --bare");

            if (!output.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
            }

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Clone(string uri)
        {
            // create the cmd to clone the repository
            var cmd = $"clone {uri} .";

            // execute the cmd
            var output = this.Execute(cmd);

            if (!output.Success)
            {
                this.logger.LogWarning(output.Error);
            }

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
            var output = this.Execute($@"add ""{spec}""");

            if (!output.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
            }

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

            var output = this.Execute(cmd);

            if (!output.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
            }

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Checkout(string name)
        {
            // checkout the branch
            var output = this.Execute($"checkout {name}");

            // determine if the operation was successful
            if (!output.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
            }

            // write the message and warning
            this.logger.LogMessage(output.Output);
            this.logger.LogWarning(output.Error);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Commit(string message)
        {
            var output = this.Execute($@"commit --allow-empty -m ""{message}""");

            if (!output.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
            }

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Tag(string name)
        {
            var exec = this.Execute($"tag {name}");

            if (!exec.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, exec.Error));
            }

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized RestoreSubmodules(bool recursive)
        {
            var cmd = "submodule update --init";

            if (recursive)
            {
                cmd += " --recursive";
            }

            this.Execute(cmd);

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
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    errorQueue.Enqueue(args.Data);
                }
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    outputQueue.Enqueue(args.Data);
                }
            };

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

                // wait for the process to truly end
                while (!process.HasExited) { }

                // capture the exit code
                exitCode = process.ExitCode;

                // determine if the process did not exit successfully
                if (exitCode != 0)
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
        public GitLog Log(string from, string to, IGitLogOptions options, IGitLogParser parser)
        {
            // create the range
            var range = from ?? (from = string.Empty);

            // determine if to is specified
            if (!string.IsNullOrEmpty(to))
            {
                if (range.Length > 0)
                {
                    range += "..";
                }

                range += to;
            }

            var cmd = $@"log {range} --format=""{Format}""";

            // create the command used to get the history of commits
            var exec = this.Execute(cmd);

            // log the output
            this.logger.LogMessage(exec.Output);

            // determine if we were successful
            if (!exec.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, exec.Error));
            }

            // parse the output
            var log = parser.Parse(GetCommits(exec.Output), options);

            // set the from/to ranges
            log.From = from ?? "<earliest>";
            log.To = to ?? "<latest>";

            // return the log
            return log;
        }

        /// <inheritdoc />
        public string RevParse(string reference)
        {
            var output = this.Execute($"rev-parse {reference}");

            if (output.Success)
            {
                return output.Output.FirstOrDefault();
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized AddRemote(string name, string uri)
        {
            // create the cmd
            var cmd = $"remote add {name} {uri}";

            // execute the cmd
            var output = this.Execute(cmd);

            // determine if we were not successful
            if (!output.Success)
            {
                // log the output
                this.logger.LogWarning(output.Error);

                // return self
                return this;
            }

            // log the output
            this.logger.LogMessage(output.Output);

            // return self
            return this;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized SetRemoteUrl(string name, string uri)
        {
            // create the cmd
            var cmd = $"remote set-url {name} {uri}";

            // execute the cmd
            var output = this.Execute(cmd);

            // determine if we were not successful
            if (!output.Success)
            {
                // log the output
                this.logger.LogWarning(output.Error);

                // move on immediately
                return this;
            }

            // log the output
            this.logger.LogMessage(output.Output);

            // return self
            return this;
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

            // this.path.Dispose();
        }

        private static IEnumerable<IList<string>> GetCommits(IEnumerable<string> lines)
        {
            var set = new List<string>();

            foreach (var line in lines)
            {
                if (Split.Equals(line))
                {
                    yield return set;

                    set = new List<string>();

                    continue;
                }

                set.Add(line);
            }
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
