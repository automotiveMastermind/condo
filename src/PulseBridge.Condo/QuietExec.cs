namespace PulseBridge.Condo
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Tasks;

    public sealed class QuietExec : Exec
    {
        protected override MessageImportance StandardOutputLoggingImportance { get; } = MessageImportance.Low;

        protected override MessageImportance StandardErrorLoggingImportance { get; } = MessageImportance.Low;
    }
}