namespace PulseBridge.Condo
{
    using System;

    /// <summary>
    /// Represents an enumeration of the possible platform types supported by CoreCLR.
    /// </summary>
    [Flags]
    public enum PlatformType
    {
        /// <summary>
        /// The platform for Windows.
        /// </summary>
        Windows = 1,

        /// <summary>
        /// The platform for Linux.
        /// </summary>
        Linux = 2,

        /// <summary>
        /// The platform for macOS.
        /// </summary>
        MacOS = 4,

        /// <summary>
        /// The platform for FreeBSD.
        /// </summary>
        FreeBSD = 8,

        /// <summary>
        /// The platform for NetBSD.
        /// </summary>
        NetBSD = 16,

        /// <summary>
        /// The platform for any flavor of Unix, which includes:
        /// <list type="bullet">
        ///     <item><see cref="PlatformType.FreeBSD"/></item>
        ///     <item><see cref="PlatformType.Linux"/></item>
        ///     <item><see cref="PlatformType.MacOS"/></item>
        ///     <item><see cref="PlatformType.NetBSD"/></item>
        /// </list>
        /// </summary>
        UnixFlavor = FreeBSD | Linux | MacOS | NetBSD,

        /// <summary>
        /// The platform is anything.
        /// </summary>
        Any = ~0
    }
}