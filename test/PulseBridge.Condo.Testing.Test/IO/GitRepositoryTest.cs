namespace PulseBridge.Condo.IO
{
    using System.IO;

    using Xunit;

    [Class(nameof(GitRepository))]
    public class GitRepositoryTest
    {
        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenPathUndefined_CreatesTemporaryPath()
        {
            // arrange
            // act
            var actual = new GitRepository();

            // assert
            Assert.NotNull(actual.ClientVersion);
            Assert.Null(actual.CurrentBranch);
            Assert.NotNull(actual.RepositoryPath);
            Assert.True(Directory.Exists(actual.RepositoryPath));
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenPathIsNotRepo_DoesNotSetCurrentBranch()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var path = temp.FullPath;

                // act
                var actual = new GitRepository(path);

                // assert
                Assert.NotNull(actual.ClientVersion);
                Assert.Null(actual.CurrentBranch);
                Assert.Equal(path, actual.RepositoryPath);
            }
        }
    }
}