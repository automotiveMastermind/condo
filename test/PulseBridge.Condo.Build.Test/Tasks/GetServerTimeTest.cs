namespace PulseBridge.Condo.Build.Tasks
{
    using Microsoft.Build.Framework;

    using Moq;

    using Xunit;

    public class GetServerTimeTest
    {
        [Fact]
        [Priority(2)]
        public void Execute_Succeeds()
        {
            // arrange
            var engine = Mock.Of<IBuildEngine>();
            var actual = new GetServerTime()
            {
                BuildEngine = engine
            };

            // act
            actual.Execute();

            // assert
            Assert.False(actual.Log.HasLoggedErrors);
        }
    }
}