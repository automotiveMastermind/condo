namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the properties and methods required to implement a git commit.
    /// </summary>
    public interface IGitCommit
    {
        #region Properties
        /// <summary>
        /// Gets or sets the hash of the commit.
        /// </summary>
        string Hash { get; set; }

        /// <summary>
        /// Gets or sets the full message associated with the git commit.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the subject (first line) of the git commit message.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body of the git commit.
        /// </summary>
        string Body { get; set; }

        /// <summary>
        /// Gets or sets the notes of the git commit.
        /// </summary>
        string Notes { get; set; }

        /// <summary>
        /// Gets the header correspondence for the git commit.
        /// </summary>
        IDictionary<string, string> HeaderCorrespondence { get; }

        /// <summary>
        /// Gets the merge correspondence for the git commit.
        /// </summary>
        IDictionary<string, string> MergeCorrespondence { get; }

        /// <summary>
        /// Gets the note correspondence for the git commit.
        /// </summary>
        IDictionary<string, string> NoteCorrespondence { get; }

        /// <summary>
        /// Gets the references associated with the git commit.
        /// </summary>
        IEnumerable<IGitReference> References { get; }
        #endregion
    }
}