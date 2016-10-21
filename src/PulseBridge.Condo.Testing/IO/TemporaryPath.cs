namespace PulseBridge.Condo.IO
{
    using System;
    using System.IO;

    using static System.FormattableString;

    /// <summary>
    /// Represents a temporary path used for testing purposes.
    /// </summary>
    public class TemporaryPath : IPathManager, IDisposable
    {
        #region Fields
        private readonly string path;

        private bool disposed;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryPath"/> class.
        /// </summary>
        public TemporaryPath()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryPath"/> class.
        /// </summary>
        /// <param name="prefix">
        /// The prefix of the temporary path.
        /// </param>
        public TemporaryPath(string prefix)
        {
            // determine if the prefix is set
            if (string.IsNullOrEmpty(prefix))
            {
                // set a default prefix
                prefix = "condo";
            }

            // create the temporary path
            this.path = Path.Combine(Path.GetTempPath(), Invariant($"{prefix}"), Path.GetRandomFileName());

            // create the directory
            Directory.CreateDirectory(this.path);
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public string FullPath => this.path;
        #endregion

        #region Methods
        /// <summary>
        /// Combines the current temporary path with the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path that should be combined with the current temporary path.
        /// </param>
        /// <returns>
        /// The specified <paramref name="path"/> combined with the current temporary path.
        /// </returns>
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
                // throw an object disposed exception
                throw new ObjectDisposedException(nameof(TemporaryPath));
            }

            // set the disposed flag
            this.disposed = true;

            try
            {
                // delete the directory
                Directory.Delete(this.path, recursive: true);
            }
            catch
            {
                // swallow exceptions on dispose
            }
        }
        #endregion
    }
}