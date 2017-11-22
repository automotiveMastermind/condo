// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Build.Evaluation;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="ICollection{T}"/> <see langword="interface"/> used to
    /// retrieve evaluated values from a collection of <see cref="ProjectProperty"/> properties.
    /// </summary>
    public static class CollectionExtensions
    {
        #region Methods
        /// <summary>
        /// Attempt to retrieve the evaluated value for the property with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="properties">
        /// The collection of properties from which to retrieve the evaluated value.
        /// </param>
        /// <param name="name">
        /// The name of the property for which to retrieve the evaluated value.
        /// </param>
        /// <returns>
        /// The evaluated value for the property with the specified <paramref name="name"/>, if available; otherwise,
        /// <see langword="null"/>.
        /// </returns>
        public static string GetEvaluatedValue(this ICollection<ProjectProperty> properties, string name)
            => string.IsNullOrEmpty(name)
                ? null
                : properties.FirstOrDefault(property => property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    ?.EvaluatedValue;

        /// <summary>
        /// Attempt to retrieve the evaluated value for the property with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="properties">
        /// The collection of properties from which to retrieve the evaluated value.
        /// </param>
        /// <param name="name">
        /// The name of the property for which to retrieve the evaluated value.
        /// </param>
        /// <param name="value">
        /// The evaluated value for the property with the specified <paramref name="name"/>, if available; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// A value indicating whether or not an evaluated value was available for the property with the specified
        /// <paramref name="name"/>.
        /// </returns>
        public static bool TryGetEvaluatedValue
            (this ICollection<ProjectProperty> properties, string name, out string value)
        {
            value = properties.GetEvaluatedValue(name);

            return value != null;
        }
        #endregion
    }
}
