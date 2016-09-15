namespace PulseBridge.Condo.Build
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Tasks;

    public sealed class QuietExec : Exec
    {
        protected override MessageImportance StandardOutputLoggingImportance { get; } = MessageImportance.Low;
    }
}