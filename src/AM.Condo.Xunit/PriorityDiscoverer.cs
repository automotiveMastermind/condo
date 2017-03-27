// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityDiscoverer.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Represents a trait discoverer that is used to discover traits based on the current priority.
    /// </summary>
    public class PriorityDiscoverer : ITraitDiscoverer
    {
        #region Methods
        /// <summary>
        /// Gets the traits used to isolate tests based on the specified <paramref name="traitAttribute"/>.
        /// </summary>
        /// <param name="traitAttribute">
        /// The attribute used to isolate tests.
        /// </param>
        /// <returns>
        /// The collection of traits used to isolate tests based on the specified
        /// <paramref name="traitAttribute"/>.
        /// </returns>
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            // get the priority type argument from the trait constructor
            var priority = (int)traitAttribute.GetConstructorArguments().First();

            // return a new trait for the priority
            yield return new KeyValuePair<string, string>(Constants.Priority, priority.ToString());
        }
        #endregion
    }
}
