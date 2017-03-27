// <copyright file="GetPlatformInfoTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Tasks
{
    using Xunit;

    [Class(nameof(GetPlatformInfo))]
    public class GetPlatformInfoTest
    {
        [Fact(Skip = "Trait filtering is currently broken...")]
        [Platform(PlatformType.MacOS)]
        [Priority(1)]
        [Purpose(PurposeType.Integration)]
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
            Assert.Equal(expected.Linux, actual.IsLinux);
            Assert.Equal(expected.Windows, actual.IsWindows);
            Assert.Equal(expected.MacOS, actual.IsMacOS);
            Assert.Equal(expected.PlatformName, actual.Platform);
        }

        [Fact(Skip = "Trait filtering is currently broken...")]
        [Platform(PlatformType.Windows)]
        [Priority(1)]
        [Purpose(PurposeType.Integration)]
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
            Assert.Equal(expected.Linux, actual.IsLinux);
            Assert.Equal(expected.Windows, actual.IsWindows);
            Assert.Equal(expected.MacOS, actual.IsMacOS);
            Assert.Equal(expected.PlatformName, actual.Platform);
        }

        [Fact(Skip = "Trait filtering is currently broken...")]
        [Platform(PlatformType.Linux)]
        [Priority(1)]
        [Purpose(PurposeType.Integration)]
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
            Assert.Equal(expected.Linux, actual.IsLinux);
            Assert.Equal(expected.Windows, actual.IsWindows);
            Assert.Equal(expected.MacOS, actual.IsMacOS);
            Assert.Equal(expected.PlatformName, actual.Platform);
        }
    }
}
