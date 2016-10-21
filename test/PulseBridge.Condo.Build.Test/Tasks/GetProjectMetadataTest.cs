namespace PulseBridge.Condo.Build.Tasks
{
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Build.Framework;

    using Moq;

    using Xunit;

    [Class(nameof(GetProjectMetadata))]
    public class GetProjectMetadataTest
    {
        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WithValidProject_Succeeds()
        {
            // arrange
            var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var group = "src";
            var name = "example";
            var directory = Path.Combine(temp, group, name);
            var path = Path.Combine(directory, "project.json");
            var shared = Path.Combine(directory, "shared");
            var info = Path.Combine(directory, "Properties", "Condo.AssemblyInfo.cs");

            var framework = "netcoreapp1.0";
            var tfm = $"TFM_{framework.Replace('.', '_')}";

            var expected = new Dictionary<string, string>
            {
                { "ProjectName", name },
                { "ProjectGroup", group },
                { "FullPath", path },
                { "ProjectDir", directory + Path.DirectorySeparatorChar },
                { "SharedSourcesDir", shared + Path.DirectorySeparatorChar },
                { "CondoAssemblyInfo", info },
                { "TargetFrameworks", framework },
                { tfm, "true" }
            };

            var json = $"{{ \"frameworks\": {{ \"{framework}\": {{ }} }} }}";
            Directory.CreateDirectory(directory);
            File.WriteAllText(path, json);

            var actual = new Dictionary<string, string>();
            actual.Add("FullPath", path);

            var engine = MSBuildMocks.CreateEngine();

            var item = new Mock<ITaskItem>();
            item.Setup(mock => mock.SetMetadata(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((key, value) => actual.Add(key, value));
            item.Setup(mock => mock.GetMetadata(It.IsAny<string>()))
                .Returns<string>(key => actual[key]);

            var instance = new GetProjectMetadata
            {
                BuildEngine = engine,
                Projects = new[] { item.Object }
            };

            // act
            var result = instance.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected, actual);
        }
    }
}