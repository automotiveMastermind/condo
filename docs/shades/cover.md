---
layout: docs
title: cover
group: shades
---

@{/*

cover
    Collects code coverage results for the specified target process or service.

cover_target=''
    Required. The target process for which to collect code coverage results.

cover_args=''
    The arguments to pass to the code coverage target.

cover_options='-nodefaultfilters -hideskipped:All -threshold:1'
    Additional options to pass to the code coverage process.

cover_exclude_files=''
    A semi-colon (;) delimited list of file name globbing patterns that will be excluded from code coverage results.

cover_exclude_attributes='*.ExcludeFromCodeCoverage*'
    A semi-colon (;) delimited list of attribute name globbing patterns that will be excluded from code coverage results.

cover_filter='+[*]* -[*.Tests]* -[*.Test]*'
    A semi-colon (;) delimited list of PartCover filters used to include or exclude assemblies, classes, and methods from code coverage results.

cover_register='user'
    Determines the level of registration of the code coverage profiler.

cover_show_unvisited='${ false }'
    A value indicating whether or not to list unvisted methods and classes once the coverage analysis is complete.

cover_skip_autoprops='${ true }'
    A value indicating whether or not to skip code coverage of automatic properties.

cover_path='$(working_path)'
    The path in which to execute the cover command line tool.

cover_pdb_path='$(target_test_path)'
    The path containing the PDBs used to evaluate code coverage.

cover_output_path='$(target_test_path)'
    The path where the code coverage results should be stored.

base_path='$(CurrentDirectory)'
    The base path in which to execute the code coverage tool.

working_path='$(base_path)'
    The working path in which to execute the code coverage tool.

target_path='$(base_path)/artifacts'
    The path where build artifacts and results should be stored.

target_test_path='$(target_path)/test'
    The path where test results should be stored.

cover_wait='true'
    A value indicating whether or not to wait for exit.

cover_quiet='$(Build.Log.Quiet)'
    A value indicating whether or not to avoid printing output.

*/}

use namespace = 'System'
use namespace = 'System.IO'

use import = 'Condo.Build'

dnvm-locate once='dnvm-locate'

default base_path                   = '${ Directory.GetCurrentDirectory() }'
default working_path                = '${ base_path }'
default target_path                 = '${ Build.Get("BUILD_BINARIESDIRECTORY", Path.Combine(base_path, "artifacts")) }'
default target_test_path            = '${ Build.Get("COMMON_TESTRESULTSDIRECTORY", Path.Combine(target_path, "test")) }'
default dnvm_path                   = '${ Build.GetPath("dnvm").Path }'

default cover_target                = ''
default cover_pdb_path              = ''
default cover_args                  = ''
default cover_options               = '-nodefaultfilters -hideskipped:All -threshold:1'
default cover_exclude_files         = ''
default cover_exclude_attributes    = '*.ExcludeFromCodeCoverage*'
default cover_filter                = '+[*]* -[Xunit]* -[Xunit.*]* -[xunit]* -[xunit.*]*'
default cover_register              = 'user'
default cover_show_unvisited        = '${ false }'
default cover_skip_autoprops        = '${ true }'
default cover_output_path           = '${ target_test_path }'
default cover_path                  = '${ working_path }'
default cover_wait                  = '${ true }'
default cover_quiet                 = '${ Build.Log.Quiet }'
default cover_install_path          = '${ Path.Combine(base_path, "packages") }'
default cover_dnx_runtime           = ''

var cover_id                        = 'OpenCover'
var cover_exe                       = 'OpenCover.Console.exe'
var cover_install                   = '${ Path.Combine(cover_install_path, cover_id, "tools", cover_exe) }'
var cover_exec_cmd                  = ''
var cover_where_cmd                 = ''

nuget-install nuget_install_id='${ cover_id }' nuget_install_exclude_version='${ true }' if='!File.Exists(cover_install) && !Build.Unix' nuget_install_prerelease='${ true }' once='cover-install'

@{
    Build.Log.Header("cover");

    if (string.IsNullOrEmpty(cover_target))
    {
        if (!string.IsNullOrEmpty(cover_dnx_runtime))
        {
            if (Build.Unix)
            {
                cover_exec_cmd = '/usr/bin/env';
                cover_where_cmd = string.Format(@"bash -c 'source ""{0}"" && dnvm use {1} 1>/dev/null 2>&1 && which dnx' 2>/dev/null", dnvm_path, cover_dnx_runtime);
            }
            else
            {
                cover_exec_cmd = dnvm_path;
                cover_where_cmd = string.Format(@"use {0} 1>nul 2>&1 && where dnx", cover_dnx_runtime);
            }
        }

        Build.TryExecute(cover_exec_cmd, out cover_target, cover_where_cmd);
    }

    if (string.IsNullOrEmpty(cover_target))
    {
        throw new ArgumentNullException("cover: cover_target must be specified.", "cover_target");
    }

    Build.MakeDirectory(Path.GetDirectoryName(cover_output_path));

    Build.Log.Argument("target", cover_target);
    Build.Log.Argument("arguments", cover_args);
    Build.Log.Argument("options", cover_options);
    Build.Log.Argument("excluded files", cover_exclude_files);
    Build.Log.Argument("excluded attrs", cover_exclude_attributes);
    Build.Log.Argument("filter", cover_filter);
    Build.Log.Argument("target path", cover_path);
    Build.Log.Argument("output path", cover_output_path);
    Build.Log.Argument("pdb path", cover_pdb_path);
    Build.Log.Argument("register", cover_register);
    Build.Log.Argument("show unvisited", cover_show_unvisited);
    Build.Log.Argument("skip auto-props", cover_skip_autoprops);
    Build.Log.Argument("wait", cover_wait);
    Build.Log.Argument("quiet", cover_quiet);
    Build.Log.Header();

    if (Build.Unix)
    {
        Build.Log.Warn("cover: code coverage analysis is only available on the Windows platform at the present time.");
    }
}

exec exec_cmd='${ cover_target }' exec_args='${ cover_args }' exec_path='${ cover_path }' if='Build.Unix'

@{
    cover_options = (cover_options ?? string.Empty) + string.Format(@" -target:""{0}""", cover_target);

    if (!string.IsNullOrEmpty(cover_pdb_path) && cover_target.Contains("dnx"))
    {
        cover_args = string.Format(@" --lib {0} {1}", cover_pdb_path, cover_args).Trim();
    }

    if (!string.IsNullOrEmpty(cover_args))
    {
        cover_options += string.Format(@" -targetargs:""{0}""", cover_args.Replace("\"", "\\\""));
    }

    if (!string.IsNullOrEmpty(cover_exclude_files))
    {
        cover_options += string.Format(@" -excludebyfile:""{0}""", cover_exclude_files);
    }

    if (!string.IsNullOrEmpty(cover_exclude_attributes))
    {
        cover_options += string.Format(@" -excludebyattribute:""{0}""", cover_exclude_attributes);
    }

    if (!string.IsNullOrEmpty(cover_filter))
    {
        cover_options += string.Format(@" -filter:""{0}""", cover_filter);
    }

    if (!string.IsNullOrEmpty(cover_register))
    {
        cover_options += string.Format(@" -register:""{0}""", cover_register);
    }

    if (!string.IsNullOrEmpty(cover_output_path))
    {
        if (!cover_output_path.EndsWith("xml"))
        {
            Path.Combine(cover_output_path, "coverage.xml");
        }

        cover_options += string.Format(@" -output:""{0}""", cover_output_path);
    }

    if (cover_show_unvisited)
    {
        cover_options += " -showunvisited";
    }

    if (cover_skip_autoprops)
    {
        cover_options += " -skipautoprops";
    }

    cover_options = cover_options.Trim();
}

exec exec_cmd='${ cover_install }' exec_args='${ cover_options }' exec_path='${ cover_path }' exec_wait='${ cover_wait }' exec_quiet='${ cover_quiet }' if='!Build.Unix'