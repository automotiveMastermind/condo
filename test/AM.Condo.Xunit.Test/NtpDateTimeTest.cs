// <copyright file="NtpDateTimeTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo
{
    using System;

    using Xunit;

    [Class(nameof(NtpTimestamp))]
    [Purpose(PurposeType.Unit)]
    public class NtpDateTimeTest
    {
        [Fact]
        [Priority(2)]
        public void Ctor_WhenDateSpecifiedAsUniversalEpoch_Succeeds()
        {
            // arrange
            var ms = 0L;
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(ms);

            var expected = new
            {
                Date = date,
                Ticks = ms * 10000L,
                Seconds = ms / 1000L,
                Fraction = (long)((decimal)ms % 1000m / 1000m * 0x100000000L),
                Timestamp = new byte[8]
            };

            // act
            var actual = new NtpTimestamp(date);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenDateSpecifiedAsLocalEpoch_Succeeds()
        {
            // arrange
            var ms = 0L;
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(ms)
                .ToLocalTime();

            var expected = new
            {
                Date = date.ToUniversalTime(),
                Ticks = ms * 10000L,
                Seconds = ms / 1000L,
                Fraction = (long)((decimal)ms % 1000m / 1000m * 0x100000000L),
                Timestamp = new byte[8]
            };

            // act
            var actual = new NtpTimestamp(date);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenDateSpecifiedAsUniversal_Succeeds()
        {
            // arrange
            var ms = 1500L;
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(ms);

            var expected = new
            {
                Date = date,
                Ticks = ms * 10000L,
                Seconds = ms / 1000L,
                Fraction = (long)((decimal)ms % 1000m / 1000m * 0x100000000L),
                Timestamp = new byte[8] { 0, 0, 0, 1, 128, 0, 0, 0 }
            };

            // act
            var actual = new NtpTimestamp(date);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenDateSpecifiedAsLocal_Succeeds()
        {
            // arrange
            var ms = 1500L;
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(ms)
                .ToLocalTime();

            var expected = new
            {
                Date = date.ToUniversalTime(),
                Ticks = ms * 10000L,
                Seconds = ms / 1000L,
                Fraction = (long)((decimal)ms % 1000m / 1000m * 0x100000000L),
                Timestamp = new byte[8] { 0, 0, 0, 1, 128, 0, 0, 0 }
            };

            // act
            var actual = new NtpTimestamp(date);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTicksSpecifiedAsUniversalEpoch_Succeeds()
        {
            // arrange
            var ticks = 15000000L;
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddTicks(ticks);

            var expected = new
            {
                Date = date,
                Ticks = ticks,
                Seconds = ticks / 10000000L,
                Fraction = (long)((decimal)ticks % 1e7m / 1e7m * 0x100000000L),
                Timestamp = new byte[8] { 0, 0, 0, 1, 128, 0, 0, 0 }
            };

            // act
            var actual = new NtpTimestamp(date.ToLocalTime().Ticks);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTicksSpecifiedAsLocalEpoch_Succeeds()
        {
            // arrange
            var ticks = 0L;
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddTicks(ticks);

            var expected = new
            {
                Date = date,
                Ticks = ticks,
                Seconds = ticks / 10000000L,
                Fraction = (long)((decimal)ticks % 1e7m / 1e7m * 0x100000000L),
                Timestamp = new byte[8]
            };

            // act
            var actual = new NtpTimestamp(date.ToLocalTime().Ticks);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTicksSpecifiedAsUniversal_Succeeds()
        {
            // arrange
            var ticks = 15000000L;
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddTicks(ticks);

            var expected = new
            {
                Date = date,
                Ticks = ticks,
                Seconds = ticks / 10000000L,
                Fraction = (long)((decimal)ticks % 1e7m / 1e7m * 0x100000000L),
                Timestamp = new byte[8] { 0, 0, 0, 1, 128, 0, 0, 0 }
            };

            // act
            var actual = new NtpTimestamp(date.Ticks, DateTimeKind.Utc);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTicksSpecifiedAsLocal_Succeeds()
        {
            // arrange
            var ticks = 15000000L;
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddTicks(ticks);

            var expected = new
            {
                Date = date,
                Ticks = ticks,
                Seconds = ticks / 10000000L,
                Fraction = (long)((decimal)ticks % 1e7m / 1e7m * 0x100000000L),
                Timestamp = new byte[8] { 0, 0, 0, 1, 128, 0, 0, 0 }
            };

            // act
            var actual = new NtpTimestamp(date.ToLocalTime().Ticks);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenNtpSpecifiedAsEpoch_Succeeds()
        {
            // arrange
            var seconds = 0L;
            var fraction = 0L;

            ulong ntp = (ulong)(seconds << 32);
            ntp += (ulong)fraction;

            var ticks = (long)((decimal)ntp * 1e7m / 0x100000000L);

            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddTicks(ticks);

            var expected = new
            {
                Date = date,
                Ticks = ticks,
                Seconds = seconds,
                Fraction = fraction,
                Timestamp = new byte[8]
            };

            // act
            var actual = new NtpTimestamp(seconds, fraction);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenNtpSpecified_Succeeds()
        {
            // arrange
            var seconds = 1L;
            var fraction = 2147483648L;

            ulong ntp = (ulong)(seconds << 32);
            ntp += (ulong)fraction;

            var ticks = (long)((decimal)ntp * 1e7m / 0x100000000L);

            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddTicks(ticks);

            var expected = new
            {
                Date = date,
                Ticks = ticks,
                Seconds = seconds,
                Fraction = fraction,
                Timestamp = new byte[8] { 0, 0, 0, 1, 128, 0, 0, 0 }
            };

            // act
            var actual = new NtpTimestamp(seconds, fraction);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTimestampSpecifiedAsEpoch_Succeeds()
        {
            // arrange
            var timestamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var expected = new
            {
                Date = date,
                Ticks = 0L,
                Seconds = 0L,
                Fraction = 0L,
                Timestamp = timestamp
            };

            // act
            var actual = new NtpTimestamp(timestamp);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTimestampSpecified_Succeeds()
        {
            // arrange
            var ticks = 15000000L;
            var timestamp = new byte[8] { 0, 0, 0, 1, 128, 0, 0, 0 };

            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddTicks(15000000L);

            var expected = new
            {
                Date = date,
                Ticks = ticks,
                Seconds = 1L,
                Fraction = 2147483648L,
                Timestamp = timestamp
            };

            // act
            var actual = new NtpTimestamp(timestamp);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTimestampSpecifiedWithIndex_Succeeds()
        {
            // arrange
            var ticks = 15000000L;
            var index = 4;
            var timestamp = new byte[12] { 0, 1, 2, 3, 0, 0, 0, 1, 128, 0, 0, 0 };

            var date = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddTicks(15000000L);

            var expected = new
            {
                Date = date,
                Ticks = ticks,
                Seconds = 1L,
                Fraction = 2147483648L,
                Timestamp = new byte[8] { 0, 0, 0, 1, 128, 0, 0, 0 }
            };

            // act
            var actual = new NtpTimestamp(timestamp, index);

            // assert
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Ticks, actual.Ticks);
            Assert.Equal(expected.Seconds, actual.Seconds);
            Assert.Equal(expected.Fraction, actual.Fraction);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTimestampWithIndexBelowZero_Throws()
        {
            // arrange
            var timestamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var index = -1;

            // act
            Action act = () => new NtpTimestamp(timestamp, index);

            // assert
            Assert.Throws<ArgumentOutOfRangeException>(nameof(index), act);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTimestampWithIndexExceedsBounds_Throws()
        {
            // arrange
            var timestamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var index = timestamp.Length + 1;

            // act
            Action act = () => new NtpTimestamp(timestamp, index);

            // assert
            Assert.Throws<ArgumentOutOfRangeException>(nameof(index), act);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTimestampWithIndexWouldOverflow_Throws()
        {
            // arrange
            var timestamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var index = timestamp.Length - 7;

            // act
            Action act = () => new NtpTimestamp(timestamp, index);

            // assert
            Assert.Throws<ArgumentOutOfRangeException>(nameof(index), act);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenTimestampInvalid_Throws()
        {
            // arrange
            var timestamp = new byte[7] { 0, 0, 0, 0, 0, 0, 0 };
            var index = 0;

            // act
            Action act = () => new NtpTimestamp(timestamp, index);

            // assert
            Assert.Throws<ArgumentException>(nameof(timestamp), act);
        }
    }
}
