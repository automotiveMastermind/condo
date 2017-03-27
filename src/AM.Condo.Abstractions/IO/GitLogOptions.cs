// <copyright file="GitLogOptions.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents git log options that uses the AngularJS conventions for commits.
    /// </summary>
    public class GitLogOptions
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the regular expression pattern used to parse the subject line of a git commit.
        /// </summary>
        public string HeaderPattern { get; set; }

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a merge commit message.
        /// </summary>
        public string MergePattern { get; set; }

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a revert commit.
        /// </summary>
        public string RevertPattern { get; set; }

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a field within a commit.
        /// </summary>
        public string FieldPattern { get; set; }

        /// <summary>
        /// Gets the correspondence that should be contained within a header.
        /// </summary>
        /// <remarks>
        /// This collection should contain the same number of elements as there are capture groups in the related
        /// <see cref="HeaderPattern"/>.
        /// </remarks>
        public List<string> HeaderCorrespondence { get; } = new List<string>();

        /// <summary>
        /// Gets the correspondence that should be contained within a merge commit.
        /// </summary>
        /// <remarks>
        /// This collection should contain the same number of elements as there are capture groups in the related
        /// <see cref="MergePattern"/>.
        /// </remarks>
        public List<string> MergeCorrespondence { get; } = new List<string>();

        /// <summary>
        /// Gets the correspondence that should be contained within a revert commit.
        /// </summary>
        /// <remarks>
        /// This collection should contain the same number of elements as there are capture groups in the related
        /// <see cref="RevertPattern"/>.
        /// </remarks>
        public List<string> RevertCorrespondence { get; } = new List<string>();

        /// <summary>
        /// Gets the prefixes used to denote a reference within a git commit message.
        /// </summary>
        public List<string> ReferencePrefixes { get; } = new List<string>();

        /// <summary>
        /// Gets the prefixes used to denote a mention within a git commit message.
        /// </summary>
        public List<string> MentionPrefixes { get; } = new List<string>();

        /// <summary>
        /// Gets the actions used to denote why a reference within the git commit exists.
        /// </summary>
        public List<string> ActionKeywords { get; } = new List<string>();

        /// <summary>
        /// Gets the keywords used to denote a note or breaking change within a commit.
        /// </summary>
        public List<string> NoteKeywords { get; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether or not to include invalid commits.
        /// </summary>
        public bool IncludeInvalidCommits { get; set; }

        /// <summary>
        /// Gets or sets the header correspondence used to group commits by type.
        /// </summary>
        public string GroupBy { get; set; }

        /// <summary>
        /// Gets or sets the header correspondence used to sort commits by scope.
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the prefix used for version tags.
        /// </summary>
        public string VersionTagPrefix { get; set; }
        #endregion
    }
}
