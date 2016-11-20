namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the properties and methods required to implement options used for a git log.
    /// </summary>
    public interface IGitLogOptions
    {
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
        /// Gets the fields that should be contained within a header.
        /// </summary>
        /// <remarks>
        /// This collection should contain the same number of elements as there are capture groups in the related
        /// <see cref="HeaderPattern"/>.
        /// </remarks>
        ICollection<string> HeaderFields { get; }

        /// <summary>
        /// Gets the fields that should be contained within a merge commit.
        /// </summary>
        /// <remarks>
        /// This collection should contain the same number of elements as there are capture groups in the related
        /// <see cref="MergePattern"/>.
        /// </remarks>
        ICollection<string> MergeFields { get; }

        /// <summary>
        /// Gets the prefixes used to denote a reference within a git commit message.
        /// </summary>
        ICollection<string> ReferencePrefixes { get; }

        /// <summary>
        /// Gets the actions used to denote why a reference within the git commit exists.
        /// </summary>
        ICollection<string> ActionKeywords { get; }

        /// <summary>
        /// Gets the keywords used to denote a note or breaking change within a commit.
        /// </summary>
        ICollection<string> NoteKeywords { get; }
    }
}