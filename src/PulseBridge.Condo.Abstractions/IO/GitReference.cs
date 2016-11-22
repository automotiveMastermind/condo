namespace PulseBridge.Condo.IO
{
    /// <summary>
    /// Represents a reference to a work item, issue, pull request, or other artifact within a git commit.
    /// </summary>
    public class GitReference
    {
        #region Properties
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string Action { get; set; }
        #endregion
    }
}