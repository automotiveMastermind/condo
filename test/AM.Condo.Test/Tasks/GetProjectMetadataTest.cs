// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProjectMetadataTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Build.Framework;

    using Moq;

    using Xunit;
    using Xunit.Abstractions;

    [Class(nameof(GetProjectMetadata))]
    public class GetProjectMetadataTest
    {
        private readonly ITestOutputHelper output;

        public GetProjectMetadataTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WithValidProjectJson_Succeeds()
        {
            // arrange
            var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var group = "src";
            var name = "example";
            var directory = Path.Combine(temp, group, name);
            var path = Path.Combine(directory, "project.json");
            var shared = Path.Combine(directory, "shared");
            var info = Path.Combine(directory, "Properties", "Condo.AssemblyInfo.cs");

            var netcore10 = "netcoreapp1.0";
            var netcore11 = "netcoreapp1.1";
            var frameworks = $"{netcore11};{netcore10}";
            var extension = Path.GetExtension(path);

            var expected = new Dictionary<string, string>
            {
                { "ProjectName", name },
                { "ProjectGroup", group },
                { "FullPath", path },
                { "ProjectDir", directory + Path.DirectorySeparatorChar },
                { "SharedSourcesDir", shared + Path.DirectorySeparatorChar },
                { "CondoAssemblyInfo", info },
                { "TargetFrameworks", frameworks },
                { "NetCoreFramework", netcore11 },
                { "Extension", extension },
                { "DotNetType", "ProjectJson" }
            };

            var json = $"{{ \"frameworks\": {{ \"{netcore10}\": {{ }}, \"{netcore11}\": {{ }} }} }}";
            Directory.CreateDirectory(directory);
            File.WriteAllText(path, json);

            var actual = new Dictionary<string, string>();
            actual.Add("FullPath", path);
            actual.Add("Extension", extension);

            var engine = MSBuildMocks.CreateEngine();

            var item = new Mock<ITaskItem>();
            item.Setup(mock => mock.SetMetadata(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((key, value) => actual[key] = value);
            item.Setup(mock => mock.GetMetadata(It.IsAny<string>()))
                .Returns<string>(key => actual.ContainsKey(key) ? actual[key] : null);

            var instance = new GetProjectMetadata
            {
                BuildEngine = engine,
                Projects = new[] { item.Object }
            };

            // act
            var result = instance.Execute();

            foreach (var line in engine.Log)
            {
                this.output.WriteLine(line);
            }

            // assert
            Assert.True(result);
            Assert.Equal(expected, actual);
        }
    }
}
