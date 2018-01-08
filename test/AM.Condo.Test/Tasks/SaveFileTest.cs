// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileTest.cs" company="automotiveMastermind and contributors">
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

    [Priority(3)]
    [Purpose(PurposeType.Unit)]
    public class SaveFileTest
    {
        [Fact]
        public void Execute_WhenFileDoesNotExistAndNoContent_SavesEmptyFile()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var repositoryRoot = temp.FullPath;
                var filePath = nameof(Execute_WhenFileDoesNotExistAndNoContent_SavesEmptyFile);
                var content = default(string);
                var replace = true;

                var target = new SaveFile
                {
                    RepositoryRoot = repositoryRoot,
                    FilePath = filePath,
                    Content = content,
                    Replace = replace
                };

                // act
                var result = target.Execute();

                // assert
                Assert.True(result);
                Assert.True(File.Exists(target.FilePath));

                var actualContent = File.ReadAllText(target.FilePath);
                Assert.Equal(string.Empty, actualContent);
            }
        }

        [Fact]
        public void Execute_WhenFileDoesNotExist_SavesFileWithContent()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var repositoryRoot = temp.FullPath;
                var path = nameof(Execute_WhenFileDoesNotExist_SavesFileWithContent);
                var content = "Bar";
                var replace = true;

                var target = new SaveFile
                {
                    RepositoryRoot = repositoryRoot,
                    FilePath = path,
                    Content = content,
                    Replace = replace
                };

                // act
                var result = target.Execute();

                // assert
                Assert.True(result);
                Assert.True(File.Exists(target.FilePath));

                var actualContent = File.ReadAllText(target.FilePath);
                Assert.Equal(content, actualContent);
            }
        }

        [Fact]
        public void Execute_WhenFileExistsAndReplacementAllowed_ReplacesFileWithContent()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var repositoryRoot = temp.FullPath;
                var path = nameof(Execute_WhenFileExistsAndReplacementAllowed_ReplacesFileWithContent);
                var content = "Bar";
                var replace = true;

                File.Create(Path.Combine(repositoryRoot, path)).Dispose();

                var target = new SaveFile
                {
                    RepositoryRoot = repositoryRoot,
                    FilePath = path,
                    Content = content,
                    Replace = replace
                };

                // act
                var result = target.Execute();

                // assert
                Assert.True(result);
                Assert.True(File.Exists(target.FilePath));

                var actualContent = File.ReadAllText(target.FilePath);
                Assert.Equal(content, actualContent);
            }
        }

        [Fact]
        public void Execute_WhenFileExistsAndReplacementNotAllowed_LogsWarning()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var repositoryRoot = temp.FullPath;
                var path = nameof(Execute_WhenFileExistsAndReplacementNotAllowed_LogsWarning);
                var content = default(string);
                var replace = false;

                File.Create(Path.Combine(repositoryRoot, path)).Dispose();

                var engine = new Mock<IBuildEngine>();

                engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()));

                var target = new SaveFile
                {
                    BuildEngine = engine.Object,
                    RepositoryRoot = repositoryRoot,
                    FilePath = path,
                    Content = content,
                    Replace = replace
                };

                // act
                var result = target.Execute();

                // assert
                Assert.True(result);
                engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Exactly(1));
            }
        }
    }
}
