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
        /// <summary>
        /// Gets or sets the hash of the git commit.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the short hash of the git commit.
        /// </summary>
        public string ShortHash { get; set; }

        /// <summary>
        /// Gets or sets the raw message of the git commit.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// Gets or sets the header of the git commit.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the body of the git commit.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the footer of the git commit.
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Gets the notes associated with the git commit.
        /// </summary>
        public IList<GitNote> Notes { get; } = new List<GitNote>();

        /// <summary>
        /// Gets the header correspondence of the git commit.
        /// </summary>
        public IDictionary<string, string> HeaderCorrespondence { get; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the merge correspondence of the git commit.
        /// </summary>
        public IDictionary<string, string> MergeCorrespondence { get; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the references contained within the git commit.
        /// </summary>
        public IList<GitReference> References { get; } = new List<GitReference>();
        #endregion
    }
}