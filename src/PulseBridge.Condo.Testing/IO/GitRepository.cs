namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
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

        #region Properties
        /// <inheritdoc/>
        public string CurrentBranch { get; private set; }

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

                return result.Output;
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

                return result.Output;
            }

            set
            {
                this.Execute($@"config user.email ""{value}""");
            }
        }
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        internal GitRepository()
            : this(new TemporaryPath())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        /// <param name="path">
        /// The path containing the repository.
        /// </param>
        internal GitRepository(string path)
            : this(new PathManager(path))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        /// <param name="path">
        /// The path manager that is responsible for managing the path.
        /// </param>
        internal GitRepository(IPathManager path)
        {
            this.path = path;

            try
            {
                var version = this.Execute("--version");

                this.version = version.Output;
            }
            catch (Exception netEx)
            {
                throw new InvalidOperationException
                    (Invariant($"A git client was not found on the current path ({path.FullPath})."), netEx);
            }

            try
            {
                // get the output for the current branch
                var branch = this.Execute("symbolic-ref HEAD");

                // set the current branch
                this.CurrentBranch = branch.Output.StartsWith("refs/heads/")
                    ? branch.Output.Substring(11)
                    : branch.Output;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public IGitRepositoryInitialized Initialize()
        {
            return this.Initialize("pb-open", "open@pulsebridge.com");
        }

        /// <inheritdoc/>
        public IGitRepositoryBare Bare()
        {
            this.Execute("init --bare");

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Initialize(string name, string email)
        {
            // execute the init command
            this.Execute("init");

            // set the user and email
            this.Username = name;
            this.Email = email;

            // set the current branch
            this.CurrentBranch = "master";

            // return self
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
        public IGitRepositoryInitialized Save()
        {
            return this.Save("README");
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Save(string relativePath)
        {
            return this.Save(relativePath, string.Empty);
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Save(string relativePath, string contents)
        {
            // save the contents using the path manager
            this.path.Save(relativePath, contents);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Add()
        {
            return this.Add(".");
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Add(string spec)
        {
            this.Execute($@"add ""{spec}""");

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Commit(string subject)
        {
            return this.Commit(type: null, scope: null, subject: subject, body: null);
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Commit(string type, string subject)
        {
            return this.Commit(type, scope: null, subject: subject, body: null);
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Commit(string type, string scope, string subject)
        {
            return this.Commit(type, scope, subject, body: null);
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Commit(string type, string scope, string subject, string body)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject), $"The {nameof(subject)} parameter cannot be null.");
            }

            if (subject == string.Empty)
            {
                throw new ArgumentException(nameof(subject), $"The {nameof(subject)} parameter cannot be empty.");
            }

            var message = subject;

            if (type != null)
            {
                message = scope == null ? $"{type}: {message}" : $"{type}({scope}): {message}";
            }

            var cmd = $@"commit --allow-empty -m ""{message}""";

            if (body != null)
            {
                cmd += $@" -m ""{body}""";
            }

            this.Execute(cmd);

            return this;
        }


        /// <inheritdoc/>
        public IGitRepositoryInitialized Branch(string name)
        {
            return this.Branch(name, source: null);
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

            this.CurrentBranch = name;

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Checkout(string name)
        {
            this.Execute($"checkout {name}");

            this.CurrentBranch = name;

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

            // setup event handlers to handle queue processing
            process.ErrorDataReceived += (sender, args) => errorQueue.Enqueue(args.Data);
            process.OutputDataReceived += (sender, args) => outputQueue.Enqueue(args.Data);

            // start the process
            process.Start();

            // do not write to standard input
            process.StandardInput.Dispose();

            // wait for exit
            process.WaitForExit();

            // spinwait to ensure exit
            SpinWait.SpinUntil(() => process.HasExited);

            // create strings for outputs
            var error = string.Join(string.Empty, errorQueue);
            var output = string.Join(string.Empty, outputQueue);

            // determine if the process did not exit successfully
            if (process.ExitCode != 0)
            {
                // return the result
                throw new InvalidOperationException(Invariant($"Execution of {command} failed in path {this.RepositoryPath}. Error: {error}"));
            }

            // return process output
            return new ProcessOutput(output, error);
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
                throw new ArgumentException($"The {nameof(root)} path does not contain the condo template.", nameof(root));
            }

            foreach (var source in Directory.GetFiles(template))
            {
                var destination = Path.Combine(this.RepositoryPath, Path.GetFileName(source));

                File.Copy(source, destination);
            }

            File.Copy(Path.Combine(root, "condo.build"), Path.Combine(this.RepositoryPath, "condo.build"));

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