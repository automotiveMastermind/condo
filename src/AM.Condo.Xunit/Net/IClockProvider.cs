// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClockProvider.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Net
{
    using System;

    /// <summary>
    /// Defines the properties and methods required to implement a provider used to retrieve the
    /// current date and time (in UTC) from a trusted authority.
    /// </summary>
    public interface IClockProvider
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets the current date and time (in UTC) from a trusted authority.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current date and time (in UTC) from a trusted authority represented as an NTP timestamp.
        /// </summary>
        NtpTimestamp NtpNow { get; }

        /// <summary>
        /// Gets the name of the authority that provided the clock.
        /// </summary>
        string Authority { get; }
        #endregion
    }
}
