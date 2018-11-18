// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetMachineSettings.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;

    using NuGet.Common;
    using NuGet.Configuration;

    /// <summary>
    /// Represents the machine-wide NuGet settings on the current system.
    /// </summary>
    public class NuGetMachineSettings : IMachineWideSettings
    {
        #region Properties and Indexers
        /// <summary>
        /// The lazy initialization field containing the machine wide settings.
        /// </summary>
        private static Lazy<ISettings> settings = new Lazy<ISettings>
            (() =>
                {
                    // get the machine wide configuration directory
                    var machinePath = NuGetEnvironment.GetFolderPath(NuGetFolderPath.MachineWideConfigDirectory);

                    // load the machine wide settings
                    return NuGet.Configuration.Settings.LoadMachineWideSettings(machinePath);
                }
            );

        /// <summary>
        /// Gets the machine-wide NuGet settings for the current system.
        /// </summary>
        public ISettings Settings => settings.Value;
        #endregion
    }
}
