namespace PulseBridge.Condo.Diagnostics
{
    /// <summary>
    /// Defines the properties and methods required to implement output from a process.
    /// </summary>
    public interface IProcessOutput
    {
        #region Properties
        /// <summary>
        /// Gets the standard output.
        /// </summary>
        string Output { get; }

        /// <summary>
        /// Gets the standard error.
        /// </summary>
        string Error { get; }
        #endregion
    }
}