// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServerTimeTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System.Net;

    using Microsoft.Build.Framework;

    using Moq;
    using AM.Condo.Net;
    using Xunit;

    [Class(nameof(GetServerTime))]
    public class GetServerTimeTest
    {
        [Agent(AgentType.Local)]
        [Fact]
        [Priority(2)]
        public void Execute_WithDefaults_Succeeds()
        {
            // arrange
            var port = 8080;
            var uri = "localhost";
            var addresses = Dns.GetHostAddressesAsync(uri).Result;
            var endpoint = new IPEndPoint(addresses[0], port);

            var expected = NtpTimestamp.UtcNow;

            var engine = new Mock<IBuildEngine>();
            var clock = Mock.Of<IClockProvider>(mock => mock.NtpNow == expected);
            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

            var actual = new GetServerTime()
            {
                Port = port,
                Uri = uri,
                BuildEngine = engine.Object
            };

            // act
            using (var server = new NtpServer(port, clock))
            {
                actual.Execute();
            }

            // assert
            engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Never);
            Assert.NotNull(actual.DateTimeUtc);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WithEmptyUri_RevertsToLocalTime()
        {
            // arrange
            var uri = string.Empty;

            var engine = new Mock<IBuildEngine>();
            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

            var actual = new GetServerTime()
            {
                BuildEngine = engine.Object,
                Uri = uri
            };

            // act
            actual.Execute();

            // assert
            engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.AtLeastOnce);
            Assert.NotNull(actual.DateTimeUtc);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WithNullUri_RevertsToLocalTime()
        {
            // arrange
            var uri = default(string);

            var engine = new Mock<IBuildEngine>();
            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

            var actual = new GetServerTime()
            {
                BuildEngine = engine.Object,
                Uri = uri
            };

            // act
            actual.Execute();

            // assert
            engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.AtLeastOnce);
            Assert.NotNull(actual.DateTimeUtc);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WithInvalidUri_RevertsToLocalTime()
        {
            // arrange
            var uri = "does-not-exist";

            var engine = new Mock<IBuildEngine>();
            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

            var actual = new GetServerTime()
            {
                BuildEngine = engine.Object,
                Uri = uri
            };

            // act
            actual.Execute();

            // assert
            engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.AtLeastOnce);
            Assert.NotNull(actual.DateTimeUtc);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WithInvalidPort_RevertsToLocalTime()
        {
            // arrange
            var port = -1;

            var engine = new Mock<IBuildEngine>();
            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

            var actual = new GetServerTime()
            {
                BuildEngine = engine.Object,
                Port = port
            };

            // act
            actual.Execute();

            // assert
            engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.AtLeastOnce);
            Assert.NotNull(actual.DateTimeUtc);
        }
    }
}
