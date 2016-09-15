namespace PulseBridge.Condo.Build
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a commit message, which may follow conventional changelog guidelines.
    /// </summary>
    public class CommitMessage
    {
        #region Properties
        /// <summary>
        /// Gets or sets the type of the commit.
        /// </summary>
        public string CommitType { get; set; }

        /// <summary>
        /// Gets or sets the scope (area) of the commit.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the subject of the commit message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body of the commit message.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets the collection of issues (work items) that the commit closes.
        /// </summary>
        public ICollection<string> Closes { get; } = new List<string>();

        /// <summary>
        /// Gets or sets a summary of breaking changes.
        /// </summary>
        public string Breaks { get; set; }

        /// <summary>
        /// Gets or sets the full commit message.
        /// </summary>
        public string Message { get; set; }
        #endregion
    }
}