using Condo.MSBuild.Project;
using Xunit;

namespace Condo.MSBuild.Test
{
    public class ExampleTest
    {
        [Fact]
        public void DoSomething_ReturnsTrue()
        {
            // arrange
            var example = new Example();

            // act
            var actual = example.DoSomething();

            // assert
            Assert.True(actual);
        }
    }
}