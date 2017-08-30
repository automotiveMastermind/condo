// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskItemExtensions.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.Build.Framework;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="ITaskItem"/> interface.
    /// </summary>
    public static class TaskItemExtensions
    {
        #region Methods
        /// <summary>
        /// Sets the metadata on the specified <paramref name="item"/> for property with the specified
        /// <paramref name="name"/> if the value exists within the specified <paramref name="xml"/>.
        /// </summary>
        /// <param name="item">
        /// The item in which to set the metadata.
        /// </param>
        /// <param name="name">
        /// The name of the property to set.
        /// </param>
        /// <param name="xml">
        /// The XML that may or may not contain a property with the specified <paramref name="name"/>.
        /// </param>
        public static void SetProperty(this ITaskItem item, string name, XDocument xml)
        {
            // parse the value from the xml
            var value = xml.Descendants(name).FirstOrDefault()?.Value;

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
