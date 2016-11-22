namespace PulseBridge.Condo
{
    using System;

    using Xunit.Sdk;

    /// <summary>
    /// Represents an attribute used to associate a platform with an xunit test.
    /// </summary>
    [TraitDiscoverer("PulseBridge.Condo.PlatformDiscoverer", "PulseBridge.Condo.Xunit")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PlatformAttribute: Attribute, ITraitAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformAttribute"/> class.
        /// </summary>
        /// <param name="platform">
        /// The platform associated with the attribute.
        /// </param>
        public PlatformAttribute(PlatformType platform)
        {
            // set the platform
            this.Platform = platform;
        }

        /// <summary>
        /// Gets the platform associated with the platform attribute.
        /// </summary>
        public PlatformType Platform { get; private set; }
    }
}