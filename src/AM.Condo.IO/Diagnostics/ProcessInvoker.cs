// <copyright file="ProcessInvoker.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using static System.FormattableString;

    /// <summary>
    /// Represents an invoker used to execute sub commands for a specific command.
    /// </summary>
    public class ProcessInvoker : IProcessInvoker
    {
        #region Fields
        private readonly ILogger logger;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInvoker"/> class.
        /// </summary>
        /// <param name="workingDirectory">
        /// The directory in which to invoke the process.
        /// </param>
        /// <param name="rootCommand">
        /// The root command to invoke.
        /// </param>
        public ProcessInvoker(string workingDirectory, string rootCommand)
            : this(workingDirectory, rootCommand, subCommand: null, logger: new NoOpLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInvoker"/> class.
        /// </summary>
        /// <param name="workingDirectory">
        /// The directory in which to invoke the process.
        /// </param>
        /// <param name="rootCommand">
        /// The root command to invoke.
        /// </param>
        /// <param name="subCommand">
        /// The subcommand to invoke.
        /// </param>
        public ProcessInvoker(string workingDirectory, string rootCommand, string subCommand)
            : this(workingDirectory, rootCommand, subCommand, logger: new NoOpLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInvoker"/> class.
        /// </summary>
        /// <param name="workingDirectory">
        /// The directory in which to invoke the process.
        /// </param>
        /// <param name="rootCommand">
        /// The root command to invoke.
        /// </param>
        /// <param name="subCommand">
        /// The subcommand to invoke.
        /// </param>
        /// <param name="logger">
        /// The logger used for logging messages to output.
        /// </param>
        public ProcessInvoker(string workingDirectory, string rootCommand, string subCommand, ILogger logger)
        {
            this.WorkingDirectory = workingDirectory ?? throw new ArgumentNullException(nameof(workingDirectory));
            this.RootCommand = rootCommand ?? throw new ArgumentNullException(nameof(rootCommand));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.SubCommand = subCommand;
        }
        #endregion

        #region Properties and Indexers
        /// <inheritdoc />
        public string WorkingDirectory { get; }

        /// <inheritdoc />
        public string RootCommand { get; }

        /// <inheritdoc />
        public string SubCommand { get; }
        #endregion

        #region Methods
        /// <inheritdoc />
        public IProcessOutput Execute(string command, bool throwOnError)
        {
            var start = this.CreateProcessInfo(command);

            this.logger.LogMessage($"executing: {start.FileName} {start.Arguments}", LogLevel.High);

            var process = new Process
            {
                StartInfo = start,
                EnableRaisingEvents = true
            };

            // create queues for data
            var errorQueue = new Queue<string>();
            var outputQueue = new Queue<string>();
            var exitCode = -1;

            // setup event handlers to handle queue processing
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    errorQueue.Enqueue(args.Data);
                }
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    outputQueue.Enqueue(args.Data);
                }
            };

            try
            {
                // start the process
                process.Start();

                // start listening for events
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                // do not write to standard input
                process.StandardInput.Dispose();

                // wait for exit
                process.WaitForExit();

                // wait for the process to truly end
                while (!process.HasExited)
                {
                }

                // capture the exit code
                exitCode = process.ExitCode;

                // determine if the process did not exit successfully
                if (exitCode != 0)
                {
                    // enqueue the error
                    errorQueue.Enqueue
                        (Invariant($"Execution of {command} failed in path {this.WorkingDirectory}. Exit Code: {process.ExitCode}"));
                }
            }
            catch (Exception netEx)
            {
                errorQueue.Enqueue
                    (Invariant($"Execution of {command} failed in path {this.WorkingDirectory}. Error: {netEx.Message}"));
            }
            finally
            {
                process.Dispose();
            }

            // return process output
            var output = new ProcessOutput(outputQueue, errorQueue, exitCode);

            // determine if we were not successful
            if (!output.Success)
            {
                // log the output
                this.logger.LogWarning(output.Error);

                // determine if we should throw
                if (throwOnError)
                {
                    throw new InvalidOperationException(string.Join(Environment.NewLine, output.Error));
                }
            }
            else
            {
                // log the output
                this.logger.LogMessage(output.Output, LogLevel.Low);
            }

            // return the output
            return output;
        }

        private ProcessStartInfo CreateProcessInfo(string command)
        {
            if (!string.IsNullOrEmpty(this.SubCommand))
            {
                command = $"{this.SubCommand} {command}";
            }

            return new ProcessStartInfo(this.RootCommand, command)
            {
                WorkingDirectory = this.WorkingDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };
        }
        #endregion
    }
}
