namespace PulseBridge.Condo.Build.Tasks
{
    using System.IO;

    using Microsoft.Build.Framework;

    using Moq;

    using Xunit;

    public class GetRepositoryInfoTest
    {
        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootNull_Succeeds()
        {
            // arrange
            var root = default(string);
            var engine = Mock.Of<IBuildEngine>();

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootDoesNotExist_Fails()
        {
            // arrange
            var root = "/nowhere";
            var engine = Mock.Of<IBuildEngine>();

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootNotRepository_Fails()
        {
            // arrange
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var engine = Mock.Of<IBuildEngine>();

            Directory.CreateDirectory(root);

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootValid_Succeeds()
        {
            // arrange
            var root = Directory.GetCurrentDirectory();
            var engine = Mock.Of<IBuildEngine>();

            while (!File.Exists(Path.Combine(root, "condo.sh")))
            {
                root = Directory.GetParent(root).FullName;
            }

            var expected = new
            {
                RepositoryRoot = root
            };

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
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