namespace Condo.Project.Test
{
    using Xunit;
    
    public class CondoTest
    {
        [Fact]
        public void SimpleTest()
        {
            Assert.Equal(4, 2 + 2);
        }

        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(9)]
        [Theory]
        public void SimpleTest(int value)
        {
            Assert.True(value % 2 == 1);
        }
    }
}