// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathManagerTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;
    using System.IO;

    using Xunit;

    [Class(nameof(PathManager))]
    [Priority(2)]
    [Purpose(PurposeType.Unit)]
    public class PathManagerTest
    {
        [Fact]
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
        public void Ctor_WhenPathDoesNotExist_Throws()
        {
            // arrange
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // act
            Action act = () => new PathManager(path);

            // assert
            Assert.Throws<DirectoryNotFoundException>(act);
        }

        [Fact]
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
    }
}
