namespace PulseBridge.Condo
{
    using System;

    using Xunit.Sdk;

    /// <summary>
    /// Represents an attribute used to associate a agent type with an xunit test.
    /// </summary>
    [TraitDiscoverer("PulseBridge.Condo.AgentDiscoverer", "PulseBridge.Condo.Xunit")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AgentAttribute: Attribute, ITraitAttribute
    {
        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentAttribute"/> class.
        /// </summary>
        /// <param name="type">
        /// The agent type associated with the attribute.
        /// </param>
        public AgentAttribute(AgentType agent)
        {
            // set the fact type
            this.Agent = agent;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the agent type associated with the type attribute.
        /// </summary>
        public AgentType Agent { get; private set; }
        #endregion
    }
}