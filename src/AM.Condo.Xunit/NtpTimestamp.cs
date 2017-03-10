namespace AM.Condo
{
    using System;

    using static System.FormattableString;

    /// <summary>
    /// Represents a date and time that is stored as using the 64-bit timestamp format as defined in IETF RFC 5905
    /// section 6 (https://tools.ietf.org/html/rfc5905#section-6).
    /// </summary>
    /// <remarks>
    /// RFC 5905 indicates that the seconds portion of the 64-bit timestamp format should be represented as an unsigned
    /// 32-bit field; however, unsigned value types are not common language specification (CLS) compliant, so a 64-bit
    /// signed integer is used in its place.
    ///
    /// TODO: make convertible
    /// </remarks>
    public struct NtpTimestamp: IComparable, IComparable<NtpTimestamp>, IEquatable<NtpTimestamp>
    {
        #region Constants
        /// <summary>
        /// The default epoch of January 1, 1900 (UTC) represented in ticks.
        /// </summary>
        private const long Epoch = 599266080000000000L;
        #endregion

        #region Private Fields
        /// <summary>
        /// The date and time (UTC) represented by the NTP timestamp.
        /// </summary>
        private readonly DateTime date;

        /// <summary>
        /// An unsigned 64-bit integer that represents both the seconds and fractions fields of the 64-bit timestamp
        /// format.
        /// </summary>
        private readonly ulong ntp;

        /// <summary>
        /// The seconds portion of the 64-bit timestamp format, which is an unsigned 32-bit value contained within a
        /// CLS-compliant 64-bit integer.
        /// </summary>
        private readonly long seconds;

        /// <summary>
        /// The fractions portion of the timestamp format.
        /// </summary>
        private readonly long fraction;

        /// <summary>
        /// The number of ticks that have elapsed between the epoch and the date and time represented by the timestamp.
        /// </summary>
        private readonly long ticks;

        /// <summary>
        /// The timestamp represented by a byte-array in big-endian format.
        /// </summary>
        private readonly byte[] timestamp;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="NtpTimestamp"/> structure.
        /// </summary>
        /// <param name="date">
        /// The date and time that should be represented as an NTP timestamp.
        /// </param>
        /// <remarks>
        /// The date and time can be represented in either UTC or local time; however, the value will be stored in UTC.
        /// </remarks>
        public NtpTimestamp(DateTime date)
        {
            this.date = date.ToUniversalTime();
            this.ticks = this.date.Ticks - Epoch;
            this.ntp = TicksToNtp(this.ticks);
            this.timestamp = NtpToTimestamp(this.ntp);

            this.seconds = (long)(this.ntp >> 32);
            this.fraction = (long)(this.ntp << 32 >> 32);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpTimestamp"/> structure.
        /// </summary>
        /// <param name="ticks">
        /// A date and time expressed in the number of 100-nanosecond intervals that have elapsed since January 1, 0001
        /// at 00:00:00.000 in the Gregorian calendar.
        /// </param>
        public NtpTimestamp(long ticks)
            : this(new DateTime(ticks))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpTimestamp"/> structure.
        /// </summary>
        /// <param name="ticks">
        /// A date and time expressed in the number of 100-nanosecond intervals that have elapsed since January 1, 0001
        /// at 00:00:00.000 in the Gregorian calendar.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration values that indicates whether ticks specifies a local time, Coordinated Universal
        /// Time (UTC), or neither.
        /// </param>
        public NtpTimestamp(long ticks, DateTimeKind kind)
            : this(new DateTime(ticks, kind))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpTimestamp"/> structure.
        /// </summary>
        /// <param name="seconds">
        /// The number of seconds that have elapsed since the epoch of January 1, 1900 at 00:00:00.000 UTC.
        /// </param>
        /// <param name="fraction">
        /// The time that has elapsed represented as a fraction of a second since the last second included by
        /// <paramref name="seconds"/>.
        /// </param>
        public NtpTimestamp(long seconds, long fraction)
        {
            ulong ntp = (ulong)(seconds << 32);
            ntp += (ulong)fraction;

            this.ticks = NtpToTicks(ntp);
            this.ntp = ntp;
            this.timestamp = NtpToTimestamp(ntp);

            this.seconds = seconds;
            this.fraction = fraction;

            this.date = new DateTime(this.ticks + Epoch, DateTimeKind.Utc);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpTimestamp"/> structure.
        /// </summary>
        /// <param name="timestamp">
        /// A 64-bit (8 byte) NTP timestamp organized in big-endian fashion as specified in IETF RFC 5905 section 6
        /// (https://tools.ietf.org/html/rfc5905#section-6). The first two words represents an unsigned 32-bit seconds
        /// field. The second two words represents a 32-bit fraction field resolving 232 picoseconds. The prime epoch
        /// is January 1, 1900 at 00:00:00.000 UTC.
        /// </param>
        public NtpTimestamp(byte[] timestamp)
            : this(timestamp, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpTimestamp"/> structure.
        /// </summary>
        /// <param name="timestamp">
        /// A byte array containing a 64-bit (8 byte) NTP timestamp organized in big-endian fashion as specified in IETF
        /// RFC 5905 section 6 (https://tools.ietf.org/html/rfc5905#section-6). The first two words represents an
        /// unsigned 32-bit seconds field. The second two words represents a 32-bit fraction field resolving 232
        /// picoseconds. The prime epoch is January 1, 1900 at 00:00:00.000 UTC.
        /// </param>
        /// <param name="index">
        /// The index within the specified <paramref name="timestamp"/> that contains the RFC 5095 timestamp field.
        /// </param>
        public NtpTimestamp(byte[] timestamp, int index)
        {
            if (timestamp.Length < 8)
            {
                throw new ArgumentException
                    (Invariant($"The {nameof(timestamp)} parameter is not a valid NTP timestamp."), nameof(timestamp));
            }

            if (index < 0 || index + 8 > timestamp.Length)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(index),
                    index,
                    Invariant($"the {nameof(index)} must not exceed the bounds of the {nameof(timestamp)} array or result in an overflow.")
                );
            }

            var ts = new byte[8];
            this.timestamp = new byte[8];

            Array.Copy(timestamp, index, ts, 0, 8);
            Array.Copy(timestamp, index, this.timestamp, 0, 8);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ts);
            }

            this.ntp = BitConverter.ToUInt64(ts, 0);
            this.ticks = NtpToTicks(this.ntp);

            this.seconds = (long)(this.ntp >> 32);
            this.fraction = (long)(this.ntp << 32 >> 32);

            this.date = new DateTime(this.ticks + Epoch, DateTimeKind.Utc);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current date and time represented as an NTP timestamp.
        /// </summary>
        public static NtpTimestamp Now => new NtpTimestamp(DateTime.Now);

        /// <summary>
        /// Gets the current date and time (in UTC) represented as an NTP timestamp.
        /// </summary>
        public static NtpTimestamp UtcNow => new NtpTimestamp(DateTime.UtcNow);

        /// <summary>
        /// Gets the date and time expressed in the number of 100-nanosecond intervals that have elapsed since
        /// January 1, 0001 at 00:00:00.000 in the Gregorian calendar.
        /// </summary>
        /// <remarks>
        /// The ticks are equivelant to those used by the <see cref="DateTime.Ticks"/> property with a
        /// <see cref="DateTimeKind.Utc"/>. The ticks are not calculated using the NTP epoch.
        /// </remarks>
        public long Ticks => this.ticks;

        /// <summary>
        /// Gets the number of seconds that have elapsed since the epoch of January 1, 1900 at 00:00:00.000 UTC.
        /// </summary>
        public long Seconds => this.seconds;

        /// <summary>
        /// The time that has elapsed represented as a fraction of a second since the last second included by
        /// <see cref="NtpTimestamp.Seconds"/>.
        /// </summary>
        public long Fraction => this.fraction;

        /// <summary>
        /// The 64-bit (8 byte) NTP timestamp organized in big-endian fashion as specified in IETF RFC 5905 section 6
        /// (https://tools.ietf.org/html/rfc5905#section-6). The first two words represents an unsigned 32-bit seconds
        /// field. The second two words represents a 32-bit fraction field resolving 232 picoseconds. The prime epoch
        /// is January 1, 1900 at 00:00:00.000 UTC.
        /// </summary>
        public byte[] Timestamp => this.timestamp;

        /// <summary>
        /// The date and time in UTC represented by this instance of the <see cref="NtpTimestamp"/> structure.
        /// </summary>
        public DateTime Date => this.date;
        #endregion

        #region Methods
        /// <summary>
        /// Determines if the specified <see cref="NtpTimestamp"/> instances are equal.
        /// </summary>
        /// <param name="ts1">
        /// The first object to compare.
        /// </param>
        /// <param name="ts2">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the specified <see cref="NtpTimestamp"/> instances are equal.
        /// </returns>
        public static bool operator ==(NtpTimestamp ts1, NtpTimestamp ts2)
        {
            return ts1.ticks.Equals(ts2.ticks);
        }

        /// <summary>
        /// Determines if the specified <see cref="NtpTimestamp"/> instances are not equal.
        /// </summary>
        /// <param name="ts1">
        /// The first object to compare.
        /// </param>
        /// <param name="ts2">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the specified <see cref="NtpTimestamp"/> instances are not equal.
        /// </returns>
        public static bool operator !=(NtpTimestamp ts1, NtpTimestamp ts2)
        {
            return !ts1.ticks.Equals(ts2.ticks);
        }

        /// <summary>
        /// Determines whether or not one <see cref="NtpTimestamp"/> is earlier than the other.
        /// </summary>
        /// <param name="ts1">
        /// The first object to compare.
        /// </param>
        /// <param name="ts2">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether or not one <see cref="NtpTimestamp"/> is earlier than the other.
        /// </returns>
        public static bool operator <(NtpTimestamp ts1, NtpTimestamp ts2)
        {
            return ts1.ticks < ts2.Ticks;
        }

        /// <summary>
        /// Determines whether or not one <see cref="NtpTimestamp"/> is later than the other.
        /// </summary>
        /// <param name="ts1">
        /// The first object to compare.
        /// </param>
        /// <param name="ts2">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether or not one <see cref="NtpTimestamp"/> is later than the other.
        /// </returns>
        public static bool operator >(NtpTimestamp ts1, NtpTimestamp ts2)
        {
            return ts1.ticks > ts2.Ticks;
        }

        /// <summary>
        /// Determines whether or not one <see cref="NtpTimestamp"/> is earlier than or equal to the other.
        /// </summary>
        /// <param name="ts1">
        /// The first object to compare.
        /// </param>
        /// <param name="ts2">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether or not one <see cref="NtpTimestamp"/> is earlier than or equal to the other.
        /// </returns>
        public static bool operator <=(NtpTimestamp ts1, NtpTimestamp ts2)
        {
            return ts1.ticks <= ts2.Ticks;
        }

        /// <summary>
        /// Determines whether or not one <see cref="NtpTimestamp"/> is later than or equal to the other.
        /// </summary>
        /// <param name="ts1">
        /// The first object to compare.
        /// </param>
        /// <param name="ts2">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether or not one <see cref="NtpTimestamp"/> is later than or equal to the other.
        /// </returns>
        public static bool operator >=(NtpTimestamp ts1, NtpTimestamp ts2)
        {
            return ts1.ticks >= ts2.Ticks;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // get the hashcode from the ticks, which is as unique as we get
            return this.ticks.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.ticks.Equals((NtpTimestamp)obj);
        }

        /// <inheritdoc/>
        public bool Equals(NtpTimestamp other)
        {
            // determine if the ticks are Equals
            return this.ticks.Equals(other.ticks);
        }

        /// <inheritdoc/>
        public int CompareTo(NtpTimestamp other)
        {
            // use the ticks to do the comparison
            return this.ticks.CompareTo(other.ticks);
        }

        /// <inheritdoc/>
        int IComparable.CompareTo(object obj)
        {
            return this.CompareTo((NtpTimestamp)obj);
        }

        private static byte[] NtpToTimestamp(ulong ntp)
        {
            var data = BitConverter.GetBytes(ntp);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            return data;
        }

        private static ulong TicksToNtp(decimal ticks)
        {
            return (ulong)((ticks / 1e7m * 0x100000000L) + .5m);
        }

        private static long NtpToTicks(decimal ntp)
        {
            return (long)((ntp * 1e7m / 0x100000000L) + .5m);
        }
        #endregion
    }
}