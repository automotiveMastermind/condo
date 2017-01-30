namespace PulseBridge.Condo.ChangeLog
{
    /// <summary>
    /// Defines the properties and methods required to implement a change log writer.
    /// </summary>
    public interface IChangeLogWriter : IChangeLogWriterApplied, IChangeLogWriterCompiled, IChangeLogWriterCanCompile, IChangeLogWriterCanInitialize
    {
    }
}
