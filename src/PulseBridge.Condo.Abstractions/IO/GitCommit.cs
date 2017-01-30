namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using NuGet.Versioning;

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
        /// Gets or sets the semantic version associated with the commit.
        /// </summary>
        public SemanticVersion Version { get; set; }

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
        public ICollection<GitReference> References { get; } = new HashSet<GitReference>();

        /// <summary>
        /// Gets the collection of tags associated with the commit.
        /// </summary>
        public ICollection<GitTag> Tags { get; } = new HashSet<GitTag>();

        /// <summary>
        /// Gets the collection of branches associated with the commit.
        /// </summary>
        public ICollection<string> Branches { get; } = new HashSet<string>();
        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(this.ShortHash ?? this.Hash ?? "<hash>");

            if (this.Header != null)
            {
                builder.Append(" | ");
                builder.Append(this.Header);
            }

            return builder.ToString();
        }
    }
}
