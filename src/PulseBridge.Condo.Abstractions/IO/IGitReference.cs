namespace PulseBridge.Condo.IO
{
    /// <summary>
    /// Defines the properties and methods required to implement a reference to a work item from a git commit.
    /// </summary>
    public interface IGitReference
    {
        #region Properties
        /// <summary>
        /// Gets or sets the ID of the item that is referencecd.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the action associated with the item that is referenced.
        /// </summary>
        string Action { get; set; }
        #endregion
    }
}