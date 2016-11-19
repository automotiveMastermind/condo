namespace PulseBridge.Condo.Diagnostics
{
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
        public ProcessOutput(string output, string error, int exitCode)
        {
            this.Output = output ?? string.Empty;
            this.Error = error ?? string.Empty;
            this.ExitCode = exitCode;
        }
        #endregion

        #region Properties
        /// <inheritdoc />
        public string Error { get; private set; }

        /// <inheritdoc />
        public string Output { get; private set; }

        /// <inheritdoc />
        public int ExitCode { get; private set; }

        /// <inheritdoc />
        public bool Success => this.ExitCode == 0;
        #endregion
    }
}