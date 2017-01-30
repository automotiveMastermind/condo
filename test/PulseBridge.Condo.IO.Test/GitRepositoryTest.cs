namespace PulseBridge.Condo.IO
{
    using Xunit;

    [Class(nameof(GitRepository))]
    public class GitRepositoryTest
    {
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
                var actual = new GitRepository(temp);

                // assert
                Assert.NotNull(actual.ClientVersion);
                Assert.Null(actual.CurrentBranch);
                Assert.Equal(path, actual.RepositoryPath);
            }
        }
    }
}