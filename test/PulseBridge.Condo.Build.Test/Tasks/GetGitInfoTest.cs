namespace PulseBridge.Condo.Build.Tasks
{
    using System.IO;

    using Xunit;
    using Xunit.Abstractions;

    public class GetGitInfoTest
    {
        private ITestOutputHelper output;

        public GetGitInfoTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootNull_Fails()
        {
            // arrange
            var root = default(string);

            var actual = new GetGitInfo
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

            var actual = new GetGitInfo
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
            var name = nameof(Execute_WhenRepositoryRootNotRepository_Fails);
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(root);
            this.output.WriteLine($"{name}: creating directory {root}...");

            var actual = new GetGitInfo
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

            var actual = new GetGitInfo
            {
                RepositoryRoot = root
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.RepositoryRoot, actual.RepositoryRoot);
        }
    }
}