namespace PulseBridge.Condo
{
    using System;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="DateTime"/> structure for
    /// NTP timestamp conversion.
    /// </summary>
    public static class NtpDateTimeExtensions
    {
        #region Methods
        /// <summary>
        /// Converts the specified <paramref name="date"/> into an NTP timestamp.
        /// </summary>
        /// <param name="date">
        /// The date to convert.
        /// </param>
        /// <returns>
        /// The NTP timestamp that represents the specified <paramref name="date"/>.
        /// </returns>
        public static NtpTimestamp ToNtpTimestamp(this DateTime date)
        {
            // return a new timestamp
            return new NtpTimestamp(date);
        }
        #endregion
    }
}