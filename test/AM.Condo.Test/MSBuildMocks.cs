// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MSBuildMocks.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System.Collections.Generic;

    using Microsoft.Build.Framework;
    using Moq;

    using Xunit.Abstractions;

    public interface IBuildEngineLog : IBuildEngine4
    {
        IEnumerable<string> Log { get; }
    }

    public static class MSBuildMocks
    {
        public static IBuildEngineLog CreateEngine()
        {
            var engine = new Mock<IBuildEngineLog>();

            var queue = new Queue<string>();

            engine.Setup(mock => mock.LogCustomEvent(It.IsAny<CustomBuildEventArgs>()))
                .Callback<CustomBuildEventArgs>(args =>
                {
                    queue.Enqueue(args.Message);
                }
            );

            engine.Setup(mock => mock.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()))
                .Callback<BuildErrorEventArgs>(args =>
                {
                    queue.Enqueue(args.Message);
                }
            );

            engine.Setup(mock => mock.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()))
                .Callback<BuildMessageEventArgs>(args =>
                {
                    queue.Enqueue(args.Message);
                }
            );

            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()))
                .Callback<BuildWarningEventArgs>(args =>
                {
                    queue.Enqueue(args.Message);
                }
            );

            engine.Setup(mock => mock.Log).Returns(queue);

            return engine.Object;
        }

        public static IBuildEngine CreateEngine(ITestOutputHelper output)
        {
            var engine = new Mock<IBuildEngine4>();

            engine.Setup(mock => mock.LogCustomEvent(It.IsAny<CustomBuildEventArgs>()))
                .Callback<CustomBuildEventArgs>(args =>
                {
                    output.WriteLine($"CUSTOM: {args.Message}");
                }
            );

            engine.Setup(mock => mock.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()))
                .Callback<BuildErrorEventArgs>(args =>
                {
                    output.WriteLine($"ERROR: {args.Message}");
                }
            );

            engine.Setup(mock => mock.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()))
                .Callback<BuildMessageEventArgs>(args =>
                {
                    output.WriteLine($"MESSAGE: {args.Message}");
                }
            );

            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()))
                .Callback<BuildWarningEventArgs>(args =>
                {
                    output.WriteLine($"WARNING: {args.Message}");
                }
            );

            return engine.Object;
        }
    }
}
