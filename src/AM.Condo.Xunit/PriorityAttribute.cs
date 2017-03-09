namespace AM.Condo
{
    using System;

    using Xunit.Sdk;

    /// <summary>
    /// Represents an attribute used to associate a priority for an Xunit fact.
    /// </summary>
    [TraitDiscoverer("AM.Condo.PriorityDiscoverer", "AM.Condo.Xunit")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PriorityAttribute: Attribute, ITraitAttribute
    {
        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityAttribute"/> class.
        /// </summary>
        /// <param name="priority">
        /// The priority associated with the attribute.
        /// </param>
        public PriorityAttribute(int priority)
        {
            // set the priority
            this.Priority = priority;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the priority associated with the priority attribute.
        /// </summary>
        public int Priority { get; private set; }
        #endregion
    }
}