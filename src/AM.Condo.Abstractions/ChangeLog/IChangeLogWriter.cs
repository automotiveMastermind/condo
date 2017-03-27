// <copyright file="IChangeLogWriter.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.ChangeLog
{
    /// <summary>
    /// Defines the properties and methods required to implement a change log writer.
    /// </summary>
    public interface IChangeLogWriter : IChangeLogWriterApplied, IChangeLogWriterCompiled, IChangeLogWriterCanCompile, IChangeLogWriterCanInitialize
    {
    }
}
