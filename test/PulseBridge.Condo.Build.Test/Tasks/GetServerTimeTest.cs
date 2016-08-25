namespace PulseBridge.Condo.Build.Tasks
{
    using Microsoft.Build.Framework;

    using Moq;

    using Xunit;

    public class GetServerTimeTest
    {
        [Fact]
        [Priority(2)]
        public void Execute_WithDefaults_Succeeds()
        {
            // arrange
            var engine = new Mock<IBuildEngine>();
            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

            var actual = new GetServerTime()
            {
                BuildEngine = engine.Object
            };

            // act
            actual.Execute();

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