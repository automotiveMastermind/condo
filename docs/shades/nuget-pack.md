---
layout: docs
title: nuget-pack
group: shades
---

@{/*

nuget-pack
    Executes a nuget package manager command to pack the specified packages.

nuget_pack_options='' (Environment Variable: NUGET_PACK_OPTIONS)
    Additional options to include when executing the nuget command line tool for the pack operation.

nuget_pack_path='$(working_path)'
    The path to the nuget specification or project that should be packed or the directory in which
    a nuget specification or project should be found.

nuget_pack_version='$(AssemblyInfo.InformationalVersion)'
    The version used to override the version defined in a nuget specification.

base_path='$(CurrentDirectory)'
    The base path in which to execute the nuget command line tool.

working_path='$(base_path)'
    The working path in which to execute the nuget command line tool.

*/}

use namespace = 'System'
use namespace = 'System.IO'
use namespace = 'System.Linq'
use namespace = 'System.Threading.Tasks'

use import = 'Condo.Build'

default base_path           = '${ Directory.GetCurrentDirectory() }'
default working_path        = '${ base_path }'
default target_path         = '${ Path.Combine(base_path, "artifacts") }'
default target_package_path = '${ Path.Combine(target_path, "packages") }'

default nuget_pack_path     = '${ working_path }'

info-collect once='info-collect'

default nuget_pack_version  = '${ AssemblyInfo.InformationalVersion }'
default nuget_pack_options  = '${ Build.Get("NUGET_PACK_OPTIONS") }'

@{
    Build.Log.Header("nuget-pack");
    Build.Log.Argument("options", nuget_pack_options);
    Build.Log.Argument("path", nuget_pack_path);
    Build.Log.Argument("version", nuget_pack_version);
    Build.Log.Header();

    if (!string.IsNullOrEmpty(nuget_pack_version))
    {
        nuget_pack_options += " -Version " + nuget_pack_version;
    }

    if (!string.IsNullOrEmpty(target_package_path))
    {
        nuget_pack_options += " -OutputDirectory \"" + target_package_path + "\"";

        Directory.CreateDirectory(target_package_path);
    }

    if (Directory.Exists(nuget_pack_path))
    {
        var nuget_pack_paths = Directory.EnumerateFiles(nuget_pack_path, "*.nuspec")
            .Union(Directory.EnumerateFiles(nuget_pack_path, "*.??proj"))
            .Distinct();

        try
        {
            Parallel.ForEach(nuget_pack_paths, path => Pack("pack \"" + path + "\"", nuget_pack_options, Path.GetDirectoryName(path), Build.Log.Quiet));
        }
        catch (AggregateException agEx)
        {
            foreach(var ex in agEx.InnerExceptions)
            {
                Log.Warn("nuget-pack: " + ex.Message);
            }

            throw;
        }
    }
    else
    {
        if (!File.Exists(nuget_pack_path))
        {
            throw new ArgumentException("nuget-pack: the specified path for the specification or project was not found.", "nuget_pack_path");
        }

        Pack("pack \"" + nuget_pack_path + "\"", nuget_pack_options, Path.GetDirectoryName(nuget_pack_path), Build.Log.Quiet);
    }
}

macro name='Pack' nuget_args='string' nuget_options='string' nuget_path='string' nuget_quiet='bool'
    - nuget_options = nuget_args.EndsWith("proj") ? nuget_options + " -Symbols" : nuget_options;
    - nuget_options += " -BasePath \"" + nuget_path + "\"";
    nuget