namespace PulseBridge.Condo
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Represents a trait discoverer that is used to discover traits based on the type of agent.
    /// </summary>
    public class AgentDiscoverer : ITraitDiscoverer
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
            // get the agent type argument from the trait constructor
            var agent = (AgentType)traitAttribute.GetConstructorArguments().First();

            // determine if the type is a local agent
            if (agent.HasFlag(AgentType.Local))
            {
                yield return new KeyValuePair<string, string>(Constants.Agent, nameof(AgentType.Local));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.Agent, $"Not-{nameof(AgentType.Local)}");
            }

            // determine if the type is a continuous integration agent
            if (agent.HasFlag(AgentType.CI))
            {
                yield return new KeyValuePair<string, string>(Constants.Agent, nameof(AgentType.CI));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.Agent, $"Not-{nameof(AgentType.CI)}");
            }

            // determine if the agent has an any flag
            if (agent.HasFlag(AgentType.Any))
            {
                yield return new KeyValuePair<string, string>(Constants.Agent, nameof(AgentType.Any));
            }
        }
    }
}