namespace PulseBridge.Condo
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Represents a trait discoverer that is used to discover traits based on the type of test.
    /// </summary>
    public class FactTypeDiscoverer : ITraitDiscoverer
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
            var type = (FactType)traitAttribute.GetConstructorArguments().First();

            // determine if the type is a unit test
            if (type.HasFlag(FactType.Unit))
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, nameof(FactType.Unit));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, $"Not-{nameof(FactType.Unit)}");
            }

            // determine if the type is an end-to-end test
            if (type.HasFlag(FactType.EndToEnd))
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, nameof(FactType.EndToEnd));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, $"Not-{nameof(FactType.EndToEnd)}");
            }

            // determine if the type is an integration
            if (type.HasFlag(FactType.Integration))
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, nameof(FactType.Integration));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, $"Not-{nameof(FactType.Integration)}");
            }

            // determine if the type is a performance test
            if (type.HasFlag(FactType.Performance))
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, nameof(FactType.Performance));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.FactType, $"Not-{nameof(FactType.Performance)}");
            }
        }
    }
}