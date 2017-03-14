// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitCommand.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Commands
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a root command used to initialize a project with condo.
    /// </summary>
    public class InitCommand : IRootCommand
    {
        #region Properties and Indexers
        /// <inheritdoc />
        public string CommandName => "init";

        /// <inheritdoc />
        public string ArgumentName => null;

        /// <inheritdoc />
        public ICollection<Func<ISubCommand>> SubCommands => null;
        #endregion

        #region Methods
        /// <inheritdoc />
        public int Execute(string[] args)
        {
            return 0;
        }

        /// <summary>
        /// Runs the initialization command with the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">
        /// The arguments used to configure the initialization command.
        /// </param>
        /// <returns>
        /// A value indicating the exit code for the command.
        /// </returns>
        public static int Run(string[] args)
        {
            // create a new initialization command
            var command = new InitCommand();

            // run the command
            return command.Execute(args);
        }
        #endregion
    }
}
