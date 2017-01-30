namespace PulseBridge.Condo.IO
{
    /// <summary>
    /// Represents a note associated with a git commit.
    /// </summary>
    public class GitNote
    {
        /// <summary>
        /// Gets or sets the title of the note.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the body of the note.
        /// /// </summary>
        public string Body { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Header ?? "<unknown>";
        }
    }
}
