// <copyright file="IProcessInvoker.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Diagnostics
{
    /// <summary>
    /// Defines the properties and methods required to implement an invoker for a process.
    /// </summary>
    public interface IProcessInvoker
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets the working directory of the process invoker.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// Gets the root command for the process invoker.
        /// </summary>
        string RootCommand { get; }

        /// <summary>
        /// Gets the subcommand for the process invoker.
        /// </summary>
        string SubCommand { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="throwOnError">
        /// A value indicating whether or not to throw an exception if an error occurs.
        /// </param>
        /// <returns>
        /// The result of executing the command.
        /// </returns>
        IProcessOutput Execute(string command, bool throwOnError);
        #endregion
    }
}
