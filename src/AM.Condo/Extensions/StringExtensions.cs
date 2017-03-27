// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        private const char Delimiter = ';';

        #region Methods
        /// <summary>
        /// Splits the specified <paramref name="values"/> into an array of values using the common semi-colon delimited
        /// property list from MSBuild.
        /// </summary>
        /// <param name="values">
        /// The value to split.
        /// </param>
        /// <returns>
        /// An array of individual values retrieved from the semi-colon delimited list.
        /// </returns>
        public static string[] PropertySplit(this string values)
        {
            return (values ?? string.Empty).Trim(Delimiter).Split(Delimiter).Where(value => value.Length > 0).ToArray();
        }

        /// <summary>
        /// Joins the specified <paramref name="values"/> as a semi-colon delimited list.
        /// </summary>
        /// <param name="values">
        /// The values to join.
        /// </param>
        /// <returns>
        /// A single string represented the collection of values as a semi-colon delimited list.
        /// </returns>
        public static string PropertyJoin(this IEnumerable<string> values)
        {
            if (values == null)
            {
                return string.Empty;
            }

            return string.Join(Delimiter.ToString(), values.Select(b => b.Replace(Delimiter, ' ')));
        }
        #endregion
    }
}
