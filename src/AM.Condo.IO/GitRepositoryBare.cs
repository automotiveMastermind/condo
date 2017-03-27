// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitRepositoryBare.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;
    using System.Linq;

    using AM.Condo.Diagnostics;

    using static System.FormattableString;

    /// <summary>
    /// Represents a basic implementation of a git repository.
    /// </summary>
    public class GitRepositoryBare : IGitRepositoryBare
    {
        #region Fields
        private readonly IPathManager path;

        private readonly ILogger logger;

        private readonly Version version;

        private readonly IProcessInvoker invoker;

        private bool disposed;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepositoryBare"/> class.
        /// </summary>
        /// <param name="path">
        /// The path manager that is responsible for managing the path.
        /// </param>
        /// <param name="logger">
        /// The logger that is responsible for logging.
        /// </param>
        protected GitRepositoryBare(IPathManager path, ILogger logger)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.invoker = new ProcessInvoker(path.FullPath, "git", subCommand: null, logger: logger);

            try
            {
                // get the version outut
                var output = this.Execute("--version");

                // capture the version string
                var values = output.Output.FirstOrDefault()?.Split(new[] { '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // define a temporary value to retain the version
                var version = string.Empty;

                // iterate over each value
                foreach (var value in values)
                {
                    // determine if the segment is an int
                    if (int.TryParse(value, out int segment))
                    {
                        // add the segment to the version
                        version = $"{version}.{segment}";
                    }
                }

                // create the version
                this.version = new Version(version.Trim('.'));
            }
            catch (Exception netEx)
            {
                throw new InvalidOperationException
                    (Invariant($"A git client was not found on the current path ({path.FullPath})."), netEx);
            }
        }
        #endregion

        #region Properties and Indexers
        /// <inheritdoc/>
        public string RepositoryPath => this.path?.FullPath;

        /// <inheritdoc/>
        public Version ClientVersion => this.version;

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
        public string CurrentBranch
        {
            get
            {
                // get the output for the current branch
                var branch = this.Execute("symbolic-ref HEAD").Output.FirstOrDefault();

                // determine if the branch is specified
                if (branch == null)
                {
                    // return null
                    return null;
                }

                // return the current branch
                return branch.StartsWith("refs/heads/")
                    ? branch.Substring(11)
                    : branch;
            }
        }

        /// <summary>
        /// Gets the path manager used to manage the repository root.
        /// </summary>
        protected IPathManager PathManager => this.path;

        /// <summary>
        /// Gets the logger used to log messages.
        /// </summary>
        protected ILogger Logger => this.logger;
        #endregion

        #region Methods
        /// <inheritdoc/>
        public IProcessOutput Execute(string command, bool throwOnError = false)
        {
            // use the invoker
            return this.invoker.Execute(command, throwOnError);
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
