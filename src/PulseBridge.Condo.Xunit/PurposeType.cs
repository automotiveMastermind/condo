namespace PulseBridge.Condo
{
    using System;

    /// <summary>
    /// Represents an enumeration of the available fact (test) types supported by Xunit.
    /// </summary>
    [Flags]
    public enum PurposeType
    {
        /// <summary>
        /// The fact type is an end-to-end scenario.
        /// </summary>
        EndToEnd = 1,

        /// <summary>
        /// The fact type is a unit test.
        /// </summary>
        Unit = 2,

        /// <summary>
        /// The fact type is an integration test.
        /// </summary>
        Integration = 4,

        /// <summary>
        /// The fact type is a performance test..
        /// </summary>
        Performance = 8
    }
}