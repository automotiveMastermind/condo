namespace PulseBridge.Condo.Diagnostics
{
    using Xunit;

    [Class(nameof(ProcessOutput))]
    [Purpose(PurposeType.Unit)]
    public class ProcessOutputTest
    {
        [Fact]
        [Priority(3)]
        public void Ctor_WhenOutputNull_SetsToEmpty()
        {
            // arrange
            var output = default(string);
            var error = default(string);
            var exitCode = 0;

            // act
            var actual = new ProcessOutput(output, error, exitCode);

            // assert
            Assert.Empty(actual.Output);
        }

        [Fact]
        [Priority(3)]
        public void Ctor_WhenErrorNull_SetsToEmpty()
        {
            // arrange
            var output = default(string);
            var error = default(string);
            var exitCode = 0;

            // act
            var actual = new ProcessOutput(output, error, exitCode);

            // assert
            Assert.Empty(actual.Error);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenOutputSet_OutputMatches()
        {
            // arrange
            var output = "output";
            var error = default(string);
            var exitCode = 0;

            // act
            var actual = new ProcessOutput(output, error, exitCode);

            // assert
            Assert.Equal(output, actual.Output);
        }

        [Fact]
        [Priority(2)]
        public void Ctor_WhenErrorSet_ErrorMatches()
        {
            // arrange
            var output = default(string);
            var error = "error";
            var exitCode = 1;

            // act
            var actual = new ProcessOutput(output, error, exitCode);

            // assert
            Assert.Equal(error, actual.Error);
        }
    }
}