namespace PulseBridge.Condo.Build
{
    using System;

    using Microsoft.Build.Framework;

    using Moq;

    public static class MSBuildMocks
    {
        public static IBuildEngine4 CreateEngine()
        {
            var engine = new Mock<IBuildEngine4>();

            engine.Setup(mock => mock.LogCustomEvent(It.IsAny<CustomBuildEventArgs>()))
                .Callback<CustomBuildEventArgs>(args =>
                {
                    var message = args.Message;
                }
            );

            engine.Setup(mock => mock.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()))
                .Callback<BuildErrorEventArgs>(args =>
                {
                    var message = args.Message;
                }
            );

            engine.Setup(mock => mock.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()))
                .Callback<BuildMessageEventArgs>(args =>
                {
                    var message = args.Message;
                }
            );

            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()))
                .Callback<BuildWarningEventArgs>(args =>
                {
                    var message = args.Message;
                }
            );

            return engine.Object;
        }
    }
}