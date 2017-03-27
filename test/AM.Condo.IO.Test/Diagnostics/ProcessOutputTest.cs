// <copyright file="ProcessOutputTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Diagnostics
{
    using System.Collections.Generic;

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
            var output = default(IEnumerable<string>);
            var error = default(IEnumerable<string>);
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
            var output = default(IEnumerable<string>);
            var error = default(IEnumerable<string>);
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
            var output = new HashSet<string> { "output" };
            var error = default(IEnumerable<string>);
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
            var output = default(IEnumerable<string>);
            var error = new HashSet<string> { "error" };
            var exitCode = 1;

            // act
            var actual = new ProcessOutput(output, error, exitCode);

            // assert
            Assert.Equal(error, actual.Error);
        }
    }
}
