// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskItemExtensions.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="ITaskItem"/> interface.
    /// </summary>
    public static class TaskItemExtensions
    {
        #region Methods
        /// <summary>
        /// Sets the metadata on the specified <paramref name="item"/> for property with the specified
        /// <paramref name="name"/> if the value exists within the specified <paramref name="properties"/>.
        /// </summary>
        /// <param name="item">
        /// The item in which to set the metadata.
        /// </param>
        /// <param name="name">
        /// The name of the property to set.
        /// </param>
        /// <param name="properties">
        /// The collection of <see cref="ProjectProperty"/> properties from which to retrieve the evaluated value.
        /// </param>
        public static void SetProperty(this ITaskItem item, string name, ICollection<ProjectProperty> properties)
        {
            // attempt to get the evaluated value for the specified name
            var value = properties.GetEvaluatedValue(name);

            // determine if the value exists
            if (!string.IsNullOrEmpty(value))
            {
                // set the metadata
                item.SetMetadata(name, value);
            }
        }
        #endregion
    }
}
