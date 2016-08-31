namespace PulseBridge.Condo
{
    using System;

    using Xunit.Sdk;

    /// <summary>
    /// Represents an attribute used to associate a fact type with an xunit test.
    /// </summary>
    [TraitDiscoverer("PulseBridge.Condo.FactTypeDiscoverer", "PulseBridge.Condo.Extensions")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class FactTypeAttribute: Attribute, ITraitAttribute
    {
        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="FactTypeAttribute"/> class.
        /// </summary>
        /// <param name="type">
        /// The fact type associated with the attribute.
        /// </param>
        public FactTypeAttribute(FactType type)
        {
            // set the fact type
            this.FactType = type;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the fact type  associated with the type attribute.
        /// </summary>
        public FactType FactType { get; private set; }
        #endregion
    }
}