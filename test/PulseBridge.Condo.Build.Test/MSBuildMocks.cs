namespace PulseBridge.Condo.Build
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Build.Framework;
    using Moq;

    public static class MSBuildMocks
    {
        public static IBuildEngine4 CreateEngine()
        {
            var engine = new Mock<IBuildEngine4>();

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

            engine.Setup(mock => mock.ToString())
                .Returns(string.Join(Environment.NewLine, queue));

            return engine.Object;
        }
    }
}