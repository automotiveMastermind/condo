namespace PulseBridge.Condo.Build.Tasks
{
    using Xunit;

    public class GetPlatformInfoTest
    {
        [Fact]
        [Platform(PlatformType.MacOS)]
        public void Execute_OnMacOS_Succeeds()
        {
            // arrange
            var expected = new
            {
                Linux = false,
                Windows = false,
                MacOS = true,
                PlatformName = "macOS"
            };

            var actual = new GetPlatformInfo();

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.Linux, actual.Linux);
            Assert.Equal(expected.Windows, actual.Windows);
            Assert.Equal(expected.MacOS, actual.MacOS);
            Assert.Equal(expected.PlatformName, actual.PlatformName);
        }

        [Fact]
        [Platform(PlatformType.Windows)]
        public void Execute_OnWindows_Succeeds()
        {
            // arrange
            var expected = new
            {
                Linux = false,
                Windows = true,
                MacOS = false,
                PlatformName = "Windows"
            };

            var actual = new GetPlatformInfo();

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.Linux, actual.Linux);
            Assert.Equal(expected.Windows, actual.Windows);
            Assert.Equal(expected.MacOS, actual.MacOS);
            Assert.Equal(expected.PlatformName, actual.PlatformName);
        }

        [Fact]
        [Platform(PlatformType.Linux)]
        public void Execute_OnLinux_Succeeds()
        {
            // arrange
            var expected = new
            {
                Linux = true,
                Windows = false,
                MacOS = false,
                PlatformName = "Linux"
            };

            var actual = new GetPlatformInfo();

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.Linux, actual.Linux);
            Assert.Equal(expected.Windows, actual.Windows);
            Assert.Equal(expected.MacOS, actual.MacOS);
            Assert.Equal(expected.PlatformName, actual.PlatformName);
        }
    }
}