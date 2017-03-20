// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGitLogOptions.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the properties and methods required to implement options used for a git log.
    /// </summary>
    public interface IGitLogOptions
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the regular expression pattern used to parse the subject line of a git commit.
        /// </summary>
        string HeaderPattern { get; set; }

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a merge commit message.
        /// </summary>
        string MergePattern { get; set; }

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a revert commit.
        /// </summary>
        string RevertPattern { get; set; }

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a field within a commit.
        /// </summary>
        string FieldPattern { get; set; }

        /// <summary>
        /// Gets the correspondence that should be contained within a header.
        /// </summary>
        /// <remarks>
        /// This collection should contain the same number of elements as there are capture groups in the related
        /// <see cref="HeaderPattern"/>.
        /// </remarks>
        List<string> HeaderCorrespondence { get; }

        /// <summary>
        /// Gets the correspondence that should be contained within a revert commit.
        /// </summary>
        /// <remarks>
        /// This collection should contain the same number of elements as there are capture groups in the related
        /// <see cref="RevertPattern"/>.
        /// </remarks>
        List<string> RevertCorrespondence { get; }

        /// <summary>
        /// Gets the correspondence that should be contained within a merge commit.
        /// </summary>
        /// <remarks>
        /// This collection should contain the same number of elements as there are capture groups in the related
        /// <see cref="MergePattern"/>.
        /// </remarks>
        List<string> MergeCorrespondence { get; }

        /// <summary>
        /// Gets the prefixes used to denote a reference within a git commit message.
        /// </summary>
        List<string> ReferencePrefixes { get; }

        /// <summary>
        /// Gets the prefixes used to denote a mention within a git commit message.
        /// </summary>
        List<string> MentionPrefixes { get; }

        /// <summary>
        /// Gets the actions used to denote why a reference within the git commit exists.
        /// </summary>
        List<string> ActionKeywords { get; }

        /// <summary>
        /// Gets the keywords used to denote a note or breaking change within a commit.
        /// </summary>
        List<string> NoteKeywords { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to include invalid commits.
        /// </summary>
        bool IncludeInvalidCommits { get; set; }

        /// <summary>
        /// Gets or sets the header correspondence used to group commits by type.
        /// </summary>
        string GroupBy { get; set; }

        /// <summary>
        /// Gets or sets the header correspondence used to sort commits by scope.
        /// </summary>
        string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the prefix used for version tags.
        /// </summary>
        string VersionTag { get; set; }
        #endregion
    }
}
