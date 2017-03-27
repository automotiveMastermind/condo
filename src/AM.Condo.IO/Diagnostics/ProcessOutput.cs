// <copyright file="ProcessOutput.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Diagnostics
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a default implementation of process output.
    /// </summary>
    public class ProcessOutput : IProcessOutput
    {
        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessOutput"/> class.
        /// </summary>
        /// <param name="output">
        /// The standard output for the process.
        /// </param>
        /// <param name="error">
        /// The standard error for the process.
        /// </param>
        /// <param name="exitCode">
        /// The exit code of the process.
        /// </param>
        public ProcessOutput(IEnumerable<string> output, IEnumerable<string> error, int exitCode)
        {
            this.Output = (output ?? new HashSet<string>()).ToList().AsReadOnly();
            this.Error = (error ?? new HashSet<string>()).ToList().AsReadOnly();

            this.ExitCode = exitCode;
        }
        #endregion

        #region Properties and Indexers
        /// <inheritdoc />
        public IList<string> Error { get; private set; }

        /// <inheritdoc />
        public IList<string> Output { get; private set; }

        /// <inheritdoc />
        public int ExitCode { get; private set; }

        /// <inheritdoc />
        public bool Success => this.ExitCode == 0;
        #endregion
    }
}
