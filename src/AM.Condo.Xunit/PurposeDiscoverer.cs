// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PurposeDiscoverer.cs" company="automotiveMastermind and contributors">
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
    /// Represents a trait discoverer that is used to discover traits based on the type of test.
    /// </summary>
    public class PurposeDiscoverer : ITraitDiscoverer
    {
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
            // get the fact type argument from the trait constructor
            var type = (PurposeType)traitAttribute.GetConstructorArguments().First();

            var name = nameof(PurposeType.Unit);

            // determine if the type is a unit test
            if (type.HasFlag(PurposeType.Unit))
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, name);
                yield return new KeyValuePair<string, string>(Constants.Category, $"Purpose-{name}");
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, $"Not-{name}");
                yield return new KeyValuePair<string, string>(Constants.Category, $"Purpose-Not-{name}");
            }

            name = nameof(PurposeType.EndToEnd);

            // determine if the type is an end-to-end test
            if (type.HasFlag(PurposeType.EndToEnd))
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, name);
                yield return new KeyValuePair<string, string>(Constants.Category, $"Purpose-{name}");
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, $"Not-{name}");
                yield return new KeyValuePair<string, string>(Constants.Category, $"Purpose-Not-{name}");
            }

            name = nameof(PurposeType.Integration);

            // determine if the type is an integration
            if (type.HasFlag(PurposeType.Integration))
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, name);
                yield return new KeyValuePair<string, string>(Constants.Category, $"Purpose-{name}");
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, $"Not-{name}");
                yield return new KeyValuePair<string, string>(Constants.Category, $"Purpose-Not-{name}");
            }

            name = nameof(PurposeType.Performance);

            // determine if the type is a performance test
            if (type.HasFlag(PurposeType.Performance))
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, name);
                yield return new KeyValuePair<string, string>(Constants.Category, $"Purpose-{name}");
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, $"Not-{name}");
                yield return new KeyValuePair<string, string>(Constants.Category, $"Purpose-Not-{name}");
            }
        }
    }
}
