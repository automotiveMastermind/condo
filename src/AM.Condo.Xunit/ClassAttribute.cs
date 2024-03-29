// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassAttribute.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;

    using Xunit.Sdk;

    /// <summary>
    /// Represents an attribute used to associate a platform with an xunit test.
    /// </summary>
    [TraitDiscoverer("AM.Condo.ClassDiscoverer", "AM.Condo.Xunit")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ClassAttribute : Attribute, ITraitAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the class that is tested by the fact.
        /// </param>
        public ClassAttribute(string name)
        {
            // set the name
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the class that is tested by the fact.
        /// </summary>
        public string Name { get; private set; }
    }
}
