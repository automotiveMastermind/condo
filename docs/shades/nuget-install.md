---
layout: docs
title: nuget-install
group: shades
---

@{/*

nuget-install
    Installs a specific nuget package if it is not already installed.

nuget_install_id=''
    Required. The identifier of the package to install.

nuget_install_version=''
    The version of the package to install.

nuget_install_path='$(working_path)'
    THe path in which to execure the nuget install command.

nuget_install_options='' (Environment Variable: NUGET_INSTALL_OPTIONS)
    Additional options to include when executing the nuget install command.

nuget_install_prerelease='${ false }'
    A value indicating whether or not to install pre-release package versions.

nuget_install_exclude_version='${ true }'
    A value indicating whether or not to exclude the version from the package folder.

nuget_install_output_path='$(base_path)/packages'
    The path in which packages should be installed.

nuget_config_path='$(nuget_download_path)/nuget.config'
    The path to the nuget configuration file to use when executing nuget commands.

base_path='$(CurrentDirectory)'
    The base path in which to execute the nuget command line tool.

working_path='$(base_path)'
    The working path in which to execute the nuget command line tool.

*/}

default base_path                       = '${ Directory.GetCurrentDirectory() }'
default working_path                    = '${ base_path }'

default nuget_install_id                = ''
default nuget_install_version           = ''
default nuget_install_prerelease        = '${ false }'
default nuget_install_exclude_version   = '${ true }'
default nuget_install_options           = '${ Build.Get("NUGET_INSTALL_OPTIONS") }'
default nuget_install_path              = '${ working_path }'
default nuget_download_path             = '${ Path.Combine(base_path, ".nuget") }'
default nuget_root_config_path          = '${ Path.Combine(base_path, "nuget.config") }'
default nuget_config_path               = '${ Path.Combine(nuget_download_path, "nuget.config") }'
default nuget_install_output_path       = '${ Path.Combine(base_path, "packages") }'

@{
    Build.Log.Header("nuget-install");

    if (string.IsNullOrEmpty(nuget_install_id))
    {
        throw new ArgumentException("nuget-install: a package identifier must be specified.", "nuget_install_id");
    }

    if (!string.IsNullOrEmpty(nuget_install_options))
    {
        nuget_install_options = nuget_install_options.Trim();
    }

    Build.Log.Argument("package id", nuget_install_id);
    Build.Log.Argument("package version", nuget_install_version);
    Build.Log.Argument("pre-release", nuget_install_prerelease);
    Build.Log.Argument("exclude version", nuget_install_exclude_version);
    Build.Log.Argument("output path", nuget_install_output_path);
    Build.Log.Argument("options", nuget_install_options);

    if (File.Exists(nuget_config_path))
    {
        nuget_install_options = (nuget_install_options + " -ConfigFile " + nuget_config_path).Trim();
    }
    else if (File.Exists(nuget_root_config_path))
    {
        nuget_install_options = (nuget_install_options + " -ConfigFile " + nuget_root_config_path).Trim();
    }
    else
    {
        nuget_config_path = "";
    }

    Build.Log.Argument("config path", nuget_config_path);
    Build.Log.Argument("root config path", nuget_root_config_path);
    Build.Log.Header();

    if (!string.IsNullOrEmpty(nuget_install_version))
    {
        nuget_install_options += " -Version " + nuget_install_version;
    }

    if (nuget_install_prerelease)
    {
        nuget_install_options += " -Prerelease";
    }

    if (nuget_install_exclude_version)
    {
        nuget_install_options += " -ExcludeVersion";
    }

    if (!string.IsNullOrEmpty(nuget_install_output_path))
    {
        nuget_install_options += " -OutputDirectory \"" + nuget_install_output_path + "\"";
    }

    if (!string.IsNullOrEmpty(nuget_install_options))
    {
        nuget_install_options = nuget_install_options.Trim();
    }
}

nuget nuget_args='install "${ nuget_install_id }"' nuget_options='${ nuget_install_options }' nuget_path='${ nuget_install_path }'