namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a git commit.
    /// </summary>
    public class GitCommit
    {
        #region Properties
        /// <inheritdoc/>
        public string Hash { get; set; }

        /// <inheritdoc/>
        public string ShortHash { get; set; }

        /// <inheritdoc/>
        public string Raw { get; set; }

        /// <inheritdoc/>
        public string Subject { get; set; }

        /// <inheritdoc/>
        public string Body { get; set; }

        /// <inheritdoc/>
        public IList<GitNote> Notes { get; } = new List<GitNote>();

        /// <inheritdoc/>
        public IDictionary<string, string> HeaderCorrespondence { get; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public IDictionary<string, string> MergeCorrespondence { get; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public IList<GitReference> References { get; } = new List<GitReference>();
        #endregion
    }
}