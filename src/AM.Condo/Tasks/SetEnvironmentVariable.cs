// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetEnvironmentVariable.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Security;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that sets an environment variable to the specified value.
    /// </summary>
    public class SetEnvironmentVariable : Task
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the name of the variable to set.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the variable to set.
        /// </summary>
        [Required]
        public string Value { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="SetEnvironmentVariable"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            try
            {
                Environment.SetEnvironmentVariable(this.Name, this.Value, EnvironmentVariableTarget.Machine);
            }
            catch (ArgumentException argEx)
            {
                this.Log.LogErrorFromException(argEx);

                return false;
            }
            catch (SecurityException securityEx)
            {
                this.Log.LogErrorFromException(securityEx);

                return false;
            }

            return true;
        }
        #endregion
    }
}
