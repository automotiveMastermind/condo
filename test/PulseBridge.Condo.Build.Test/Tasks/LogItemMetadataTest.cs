namespace PulseBridge.Condo.Build.Tasks
{
    using System.Collections.Generic;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using Moq;

    using Xunit;

    public class LogItemMetadataTest
    {
        [Fact]
        [Priority(3)]
        public void Execute_WithValidItem_Succeeds()
        {
            // arrange
            var engine = new Mock<IBuildEngine>();
            var messages = new List<string>();
            var spec = "this.is.a.test";

            engine.Setup(mock => mock.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()))
                .Callback<BuildMessageEventArgs>(args => messages.Add(args.Message))
                .Verifiable();

            var metadata = new Dictionary<string, string>
            {
                { "A", "1" },
                { "B", "2" },
                { "C", "3" }
            };

            var item = new TaskItem(spec, metadata);

            var instance = new LogItemMetadata
            {
                BuildEngine = engine.Object,
                Items = new[] { item }
            };

            // act
            var result = instance.Execute();

            // assert
            Assert.True(result, "task did not execute successfully");
            engine.Verify(mock => mock.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()), Times.AtLeast(4));

            Assert.Contains(spec, messages[0]);

            foreach (var property in metadata)
            {
                Assert.True
                    (messages.Contains($"  {property.Key,-17}: {property.Value}"), $"message for {property.Key} not found.");
            }
        }
    }
}