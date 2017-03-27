// <copyright file="PlatformType.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo
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
        ///     <item><description><see cref="PlatformType.FreeBSD"/></description></item>
        ///     <item><description><see cref="PlatformType.Linux"/></description></item>
        ///     <item><description><see cref="PlatformType.MacOS"/></description></item>
        ///     <item><description><see cref="PlatformType.NetBSD"/></description></item>
        /// </list>
        /// </summary>
        UnixFlavor = FreeBSD | Linux | MacOS | NetBSD,

        /// <summary>
        /// The platform is anything.
        /// </summary>
        Any = ~0
    }
}
