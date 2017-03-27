// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuietExec.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Tasks;

    /// <summary>
    /// Represents a Microsoft Build task that will execute a child process without emitting output.
    /// </summary>
    public sealed class QuietExec : Exec
    {
        #region Properties and Indexers
        /// <inheritdoc/>
        protected override MessageImportance StandardOutputLoggingImportance { get; } = MessageImportance.Low;

        /// <inheritdoc/>
        protected override MessageImportance StandardErrorLoggingImportance { get; } = MessageImportance.Low;
        #endregion
    }
}
