// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetReleaseTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System.IO;
    using AM.Condo.IO;

    using Microsoft.Build.Framework;
    using Moq;
    using Xunit;

    [Class(nameof(GetBuildTime))]
    [Purpose(PurposeType.Unit)]
    public class GetReleaseTest
    {
        [Fact]
        [Priority(2)]
        public void Execute_WhenAssetIsGzipTarball_ThenExtracts()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var organization = "automotivemastermind";
                var repository = "condo-test";
                var tag = "v1.0-test";

                var destination = temp.FullPath;
                var asset = "release-test.tar.gz";

                var engine = new Mock<IBuildEngine>();
                engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

                var actual = new GetRelease()
                {
                    BuildEngine = engine.Object,
                    Organization = organization,
                    Repository = repository,
                    Tag = tag,
                    Destination = destination,
                    Asset = asset
                };

                var expected = temp.Combine("example/example.txt");

                // act
                actual.Execute();

                // assert
                engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Never);
                engine.Verify(mock => mock.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()), Times.Never);
                Assert.True(File.Exists(expected), $"File not found at {expected}");
            }
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenAssetIsTarball_ThenExtracts()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var organization = "automotivemastermind";
                var repository = "condo-test";
                var tag = "v1.0-test";

                var destination = temp.FullPath;
                var asset = "release-test.tar";

                var engine = new Mock<IBuildEngine>();
                engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

                var actual = new GetRelease()
                {
                    BuildEngine = engine.Object,
                    Organization = organization,
                    Repository = repository,
                    Tag = tag,
                    Destination = destination,
                    Asset = asset
                };

                var expected = temp.Combine("example/example.txt");

                // act
                actual.Execute();

                // assert
                engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Never);
                engine.Verify(mock => mock.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()), Times.Never);
                Assert.True(File.Exists(expected), $"File not found at {expected}");
            }
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenAssetIsZip_ThenExtracts()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var organization = "automotivemastermind";
                var repository = "condo-test";
                var tag = "v1.0-test";

                var destination = temp.FullPath;
                var asset = "release-test.zip";

                var engine = new Mock<IBuildEngine>();
                engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

                var actual = new GetRelease()
                {
                    BuildEngine = engine.Object,
                    Organization = organization,
                    Repository = repository,
                    Tag = tag,
                    Destination = destination,
                    Asset = asset
                };

                var expected = temp.Combine("example/example.txt");

                // act
                actual.Execute();

                // assert
                engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Never);
                engine.Verify(mock => mock.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()), Times.Never);
                Assert.True(File.Exists(expected), $"File not found at {expected}");
            }
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenAssetIsGZip_ThenExtracts()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var organization = "automotivemastermind";
                var repository = "condo-test";
                var tag = "v1.0-test";

                var destination = temp.FullPath;
                var asset = "release-test.gz";

                var engine = new Mock<IBuildEngine>();
                engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

                var actual = new GetRelease()
                {
                    BuildEngine = engine.Object,
                    Organization = organization,
                    Repository = repository,
                    Tag = tag,
                    Destination = destination,
                    Asset = asset
                };

                var expected = temp.Combine("release-test");

                // act
                actual.Execute();

                // assert
                engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Never);
                engine.Verify(mock => mock.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()), Times.Never);
                Assert.True(File.Exists(expected), $"File not found at {expected}");
            }
        }
    }
}
