namespace AM.Condo.Net
{
    using System;

    /// <summary>
    /// Represents a clock provider that uses the date and time from the local machine.
    /// </summary>
    public class LocalMachineClock : IClockProvider
    {
        #region Properties
        /// <inheritdoc/>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <inheritdoc/>
        public NtpTimestamp NtpNow => NtpTimestamp.UtcNow;

        /// <inheritdoc/>
        public string Authority => "Local Machine";
        #endregion
    }
}