namespace PulseBridge.Condo.Build.Tasks
{
    using Xunit;
    using Moq;
    using Microsoft.Build.Framework;

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

            // actual
            actual.Execute();

            // assert
            Assert.False(actual.Log.HasLoggedErrors);
        }
    }
}