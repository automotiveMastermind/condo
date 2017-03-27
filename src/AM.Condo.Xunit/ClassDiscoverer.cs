// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassDiscoverer.cs" company="automotiveMastermind and contributors">
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
    /// Represents a trait discoverer that is used to discover traits based on the class under test.
    /// </summary>
    public class ClassDiscoverer : ITraitDiscoverer
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
            // get the platform type argument from the trait constructor
            var name = (string)traitAttribute.GetConstructorArguments().First();

            // determine if the name is specified
            if (string.IsNullOrEmpty(name))
            {
                // break immediately
                yield break;
            }

            // yield the class trait
            yield return new KeyValuePair<string, string>(Constants.Class, name);
        }
        #endregion
    }
}
