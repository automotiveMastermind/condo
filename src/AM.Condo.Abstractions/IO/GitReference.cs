// <copyright file="GitReference.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.IO
{
    /// <summary>
    /// Represents a reference to a work item, issue, pull request, or other artifact within a git commit.
    /// </summary>
    public class GitReference
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the ID of the reference.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the action of the reference.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the owner of the reference.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets the repository of the reference.
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets the raw reference.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// Gets or sets the prefix of the reference.
        /// </summary>
        public string Prefix { get; set; }
        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Raw ?? "<unknown>";
        }
    }
}
