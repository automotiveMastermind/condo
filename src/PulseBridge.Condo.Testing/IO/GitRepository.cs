namespace PulseBridge.Condo.IO
{
    using System;
    using System.Diagnostics;
    using System.IO;

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
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        public GitRepository()
            : this(new TemporaryPath())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        /// <param name="path">
        /// The path containing the repository.
        /// </param>
        public GitRepository(string path)
            : this(new PathManager(path))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        /// <param name="path">
        /// The path manager that is responsible for managing the path.
        /// </param>
        public GitRepository(IPathManager path)
        {
            this.path = path;

            try
            {
                var version = this.Execute("--version");

                this.version = version.Output;
            }
            catch (Exception netEx)
            {
                throw new InvalidOperationException("A git client was not found on the current path.", netEx);
            }

            try
            {
                // get the output for the current branch
                var branch = this.Execute("symbolic-ref HEAD");

                // set the current branch
                this.CurrentBranch = branch.Output.Substring(11);
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public IGitRepository Initialize()
        {
            const string cmd = "init";

            // execute the init command
            this.Execute(cmd);

            // set the current branch
            this.CurrentBranch = "master";

            // return self
            return this;
        }

        /// <inheritdoc/>
        public IGitRepository Save()
        {
            return this.Save("README");
        }

        /// <inheritdoc/>
        public IGitRepository Save(string relativePath)
        {
            return this.Save(relativePath, string.Empty);
        }

        /// <inheritdoc/>
        public IGitRepository Save(string relativePath, string contents)
        {
            var path = this.path.Combine(relativePath);
            var directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, contents ?? string.Empty);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepository Add()
        {
            return this.Add(".");
        }

        /// <inheritdoc/>
        public IGitRepository Add(string spec)
        {
            this.Execute($@"add ""{spec}""");

            return this;
        }

        /// <inheritdoc/>
        public IGitRepository Commit(string subject)
        {
            return this.Commit(null, null, subject, null);
        }

        /// <inheritdoc/>
        public IGitRepository Commit(string type, string subject)
        {
            return this.Commit(type, null, subject, null);
        }

        /// <inheritdoc/>
        public IGitRepository Commit(string type, string scope, string subject)
        {
            return this.Commit(type, scope, subject, null);
        }

        /// <inheritdoc/>
        public IGitRepository Commit(string type, string scope, string subject, string body)
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

            if (body != null)
            {
                message = $"{message}{Environment.NewLine}{body}";
            }

            this.Execute($@"commit --allow-empty -m ""{message}""");

            return this;
        }


        /// <inheritdoc/>
        public IGitRepository Branch(string name)
        {
            return this.Branch(name, null);
        }

        /// <inheritdoc/>
        public IGitRepository Branch(string name, string source)
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
        public IGitRepository Checkout(string name)
        {
            this.Execute($"checkout {name}");

            this.CurrentBranch = name;

            return this;
        }

        /// <inheritdoc/>
        public IGitRepository Tag(string name)
        {
            this.Execute($"tag {name}");

            return this;
        }

        /// <inheritdoc/>
        public IProcessOutput Execute(string command)
        {
            var start = new ProcessStartInfo("git", command)
            {
                WorkingDirectory = this.path.FullPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(start);
            process.WaitForExit();

            if (process.HasExited && process.ExitCode == 0)
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                return new ProcessOutput(output, error);
            }

            // return the result
            throw new InvalidOperationException($"Execution of {command} failed.");
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
        #endregion
    }
}