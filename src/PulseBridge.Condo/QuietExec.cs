namespace PulseBridge.Condo
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Tasks;

    /// <summary>
    /// Represents a Microsoft Build task that will execute a child process without emitting output.
    /// </summary>
    public sealed class QuietExec : Exec
    {
        #region Properties
        /// <inheritdoc/>
        protected override MessageImportance StandardOutputLoggingImportance { get; } = MessageImportance.Low;

        /// <inheritdoc/>
        protected override MessageImportance StandardErrorLoggingImportance { get; } = MessageImportance.Low;
        #endregion
    }
}