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
        /// Sets the metadata on the specified <paramref name="item"/> for the property with the specified
        /// <paramref name="name"/> to the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="item">
        /// The item in which to set the metadata.
        /// </param>
        /// <param name="name">
        /// The name of the property to set.
        /// </param>
        /// <param name="value">
        /// The value to which to set the metadata.
        /// </param>
        public static void SetMetadata(this ITaskItem item, string name, object value)
            => item.SetMetadata(name, value?.ToString());

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
        public static void SetMetadata(this ITaskItem item, string name, ICollection<ProjectProperty> properties)
            => item.SetMetadata(name, properties, defaultValue: null);

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
        /// <param name="defaultValue">
        /// The default value if a property for the specified <paramref name="name"/> is not available.
        /// </param>
        public static void SetMetadata
            (this ITaskItem item, string name, ICollection<ProjectProperty> properties, object defaultValue)
        {
            // attempt to get the evaluated value for the specified name
            var value = properties.GetEvaluatedValue(name) ?? defaultValue?.ToString();

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
