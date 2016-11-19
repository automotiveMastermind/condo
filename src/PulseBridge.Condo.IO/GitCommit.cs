namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a git commit.
    /// </summary>
    public class GitCommit : IGitCommit
    {
        #region Properties
        /// <inheritdoc/>
        public string Hash { get; set; }

        /// <inheritdoc/>
        public string Message { get; set; }

        /// <inheritdoc/>
        public string Subject { get; set; }

        /// <inheritdoc/>
        public string Body { get; set; }

        /// <inheritdoc/>
        public string Notes { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, string> HeaderCorrespondence { get; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public IDictionary<string, string> MergeCorrespondence { get; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public IDictionary<string, string> NoteCorrespondence { get; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public IEnumerable<IGitReference> References { get; } = new HashSet<IGitReference>();
        #endregion
    }
}