// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRootCommand.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Commands
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the properties and methods required to implement a root command.
    /// </summary>
    public interface IRootCommand
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Gets the name of the argument associated with the command.
        /// </summary>
        string ArgumentName { get; }

        /// <summary>
        /// Gets the collection of sub commands associated with the command.
        /// </summary>
        ICollection<Func<ISubCommand>> SubCommands { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the command with the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">
        /// The arguments used to configure the command.
        /// </param>
        /// <returns>
        /// A value indicating the exit code of the command.
        /// </returns>
        int Execute(string[] args);
        #endregion
    }
}
