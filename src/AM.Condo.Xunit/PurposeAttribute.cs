// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PurposeAttribute.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;

    using Xunit.Sdk;

    /// <summary>
    /// Represents an attribute used to associate a fact type with an xunit test.
    /// </summary>
    [TraitDiscoverer("AM.Condo.PurposeDiscoverer", "AM.Condo.Xunit")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class PurposeAttribute : Attribute, ITraitAttribute
    {
        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="PurposeAttribute"/> class.
        /// </summary>
        /// <param name="purpose">
        /// The fact type associated with the attribute.
        /// </param>
        public PurposeAttribute(PurposeType purpose)
        {
            // set the fact type
            this.Purpose = purpose;
        }
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Gets the fact type  associated with the type attribute.
        /// </summary>
        public PurposeType Purpose { get; private set; }
        #endregion
    }
}
