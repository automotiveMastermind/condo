namespace PulseBridge.Condo.IO
{
    using System;
    using System.IO;

    using static System.FormattableString;

    /// <summary>
    /// Represents a simple manager for a path on a file system.
    /// </summary>
    public class PathManager : IPathManager
    {
        #region Private Fields
        private readonly string path;
        private bool disposed;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="PathManager"/> class.
        /// </summary>
        /// <param name="path">
        /// The path that should be managed by the path manager.
        /// </param>
        public PathManager(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), Invariant($"The {nameof(path)} cannot be null."));
            }

            if (path == string.Empty)
            {
                throw new ArgumentException(nameof(path), Invariant($"The {nameof(path)} cannot be empty."));
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException
                    (Invariant($"The specified {nameof(path)} ({path}) does not exist."));
            }

            this.path = path;
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public string FullPath => this.path;
        #endregion

        #region Methods
        /// <inheritdoc/>
        public string Combine(string path)
        {
            return Path.Combine(this.path, path);
        }

        /// <inheritdoc/>
        public bool Exists()
        {
            return Directory.Exists(this.path);
        }

        /// <inheritdoc/>
        public string Create(string relativePath)
        {
            // create the path
            var path = this.Combine(relativePath);

            // create the path
            Directory.CreateDirectory(path);

            // return the path
            return path;
        }

        /// <inheritdoc/>
        public string Save(string relativePath, string contents)
        {
            // get he final path of the file
            var path = this.Combine(relativePath);

            // get the directory name
            var directory = Path.GetDirectoryName(path);

            // determine if the directory exists
            if (!Directory.Exists(directory))
            {
                // create the directory
                Directory.CreateDirectory(directory);
            }

            // save the content
            File.WriteAllText(path, contents ?? string.Empty);

            // assume success
            return path;
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
            // determine if the object has already been disposed
            if (this.disposed)
            {
                // throw an object disposed exception
                throw new ObjectDisposedException(nameof(PathManager));
            }

            // set the disposed flag
            this.disposed = true;
        }
        #endregion
    }
}