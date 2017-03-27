// <copyright file="GitNote.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.IO
{
    /// <summary>
    /// Represents a note associated with a git commit.
    /// </summary>
    public class GitNote
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the title of the note.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the body of the note.
        /// /// </summary>
        public string Body { get; set; }
        #endregion

        #region Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return this.Header ?? "<unknown>";
        }
        #endregion
    }
}
