// <copyright file="PlatformDiscoverer.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Represents a trait discoverer that is used to discover traits based on the current platform.
    /// </summary>
    public class PlatformDiscoverer : ITraitDiscoverer
    {
        #region Methods
        /// <summary>
        /// Gets the traits used to isolate tests based on the specified <paramref name="traitAttribute"/>.
        /// </summary>
        /// <param name="traitAttribute">
        /// The attribute used to isolate tests.
        /// </param>
        /// <returns>
        /// The collection of traits used to isolate tests based on the specified
        /// <paramref name="traitAttribute"/>.
        /// </returns>
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            // get the platform type argument from the trait constructor
            var platform = (PlatformType)traitAttribute.GetConstructorArguments().First();

            // determine if the platform has a windows flag
            if (platform.HasFlag(PlatformType.Windows))
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, nameof(PlatformType.Windows));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, $"Not-{nameof(PlatformType.Windows)}");
            }

            // determine if the platform has a macos flag
            if (platform.HasFlag(PlatformType.MacOS))
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, nameof(PlatformType.MacOS));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, $"Not-{nameof(PlatformType.MacOS)}");
            }

            // determine if the platform has a linux flag
            if (platform.HasFlag(PlatformType.Linux))
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, nameof(PlatformType.Linux));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, $"Not-{nameof(PlatformType.Linux)}");
            }

            // determine if the platform has a netbsd flag
            if (platform.HasFlag(PlatformType.NetBSD))
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, nameof(PlatformType.NetBSD));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, $"Not-{nameof(PlatformType.NetBSD)}");
            }

            // determine if the platform has a freebsd flag
            if (platform.HasFlag(PlatformType.FreeBSD))
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, nameof(PlatformType.FreeBSD));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, $"Not-{nameof(PlatformType.FreeBSD)}");
            }

            // determine if the platform has a unix flag
            if (platform.HasFlag(PlatformType.UnixFlavor))
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, nameof(PlatformType.UnixFlavor));
            }
            else
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, $"Not-{nameof(PlatformType.UnixFlavor)}");
            }

            // determine if the platform has an any flag
            if (platform.HasFlag(PlatformType.Any))
            {
                yield return new KeyValuePair<string, string>(Constants.Platform, nameof(PlatformType.Any));
            }
        }
        #endregion
    }
}
