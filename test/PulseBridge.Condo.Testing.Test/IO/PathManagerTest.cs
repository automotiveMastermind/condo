namespace PulseBridge.Condo.IO
{
    using System;
    using System.IO;

    using Xunit;

    [Class(nameof(PathManager))]
    public class PathManagerTest
    {
        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenPathNull_Throws()
        {
            // arrange
            var path = default(string);

            // act
            Action act = () => new PathManager(path);

            // assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenPathEmpty_Throws()
        {
            // arrange
            var path = string.Empty;

            // act
            Action act = () => new PathManager(path);

            // assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenPathDoesNotExist_Succeeds()
        {
            // arrange
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // act
            var actual = new PathManager(path);

            // assert
            Assert.False(Directory.Exists(path));
            Assert.Equal(path, actual.FullPath);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenPathExists_Succeeds()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var path = temp.FullPath;

                // act
                var actual = new PathManager(path);

                // assert
                Assert.True(Directory.Exists(path));
                Assert.Equal(path, actual.FullPath);
            }
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Exists_WhenPathExists_ReturnsTrue()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var path = temp.FullPath;

                // act
                var actual = new PathManager(path);
                var exists = actual.Exists();

                // assert
                Assert.True(Directory.Exists(path));
                Assert.True(exists);
            }
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Exists_WhenPathDoesNotExist_ReturnsFalse()
        {
            // arrange
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // act
            var actual = new PathManager(path);
            var exists = actual.Exists();

            // assert
            Assert.False(Directory.Exists(path));
            Assert.False(exists);
        }
    }
}