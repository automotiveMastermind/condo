namespace PulseBridge.Condo.Build.Tasks
{
    using System.IO;

    using Xunit;

    public class GetRepositoryInfoTest
    {
        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootNull_Fails()
        {
            // arrange
            var root = default(string);

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.False(result);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootDoesNotExist_Fails()
        {
            // arrange
            var root = "/nowhere";

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.False(result);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootNotRepository_Fails()
        {
            // arrange
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(root);

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.False(result);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootValid_Succeeds()
        {
            // arrange
            var root = Directory.GetCurrentDirectory();

            var expected = new {
                RepositoryRoot = root
            };

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.RepositoryRoot, actual.RepositoryRoot);
            Assert.NotEqual("<unknown>", actual.Branch);
            Assert.NotNull(actual.RepositoryUri);
        }
    }
}