---
layout: docs
title: dnx
group: shades
---

@{/*

dnx
    Executes the dnx command line tool.

dnx_args=''
    The arguments for the dnx command.

dnx_runtime=''
    The runtime to use when executing dnx. This argument is passed in the raw to 'dnvm use.'

dnx_options='' (Environment Variable: DNX_OPTIONS)
    Additional options to use when executing the dnx command.

dnx_path='$(working_path)'
    The path in which to execute dnx.

base_path='$(CurrentDirectory)'
    The base path in which to execute dnx.

working_path='$(base_path)'
    The working path in which to execute dnx.

dnx_wait='true'
    A value indicating whether or not to wait for exit.

dnx_quiet='$(Build.Log.Quiet)'
    A value indicating whether or not to avoid printing output.

*/}

use import = 'Condo.Build'

dnvm-locate once='dnvm-locate'

default base_path       = '${ Directory.GetCurrentDirectory() }'
default working_path    = '${ base_path }'
default dnvm_path       = '${ Build.GetPath("dnvm").Path }'

default dnx_args        = ''
default dnx_runtime     = ''
default dnx_path        = '${ working_path }'
default dnx_options     = '${ Build.Get("DNX_OPTIONS") }'
default dnx_wait        = '${ true }'
default dnx_quiet       = '${ Build.Log.Quiet }'

@{
    Build.Log.Header("dnx");

    var dnx_exec_cmd    = "dnx";

    if (string.IsNullOrEmpty(dnx_args))
    {
        throw new ArgumentException("dnx: arguments must be specified.", "dnx_args");
    }

    dnx_args = dnx_args.Trim();

    if (!string.IsNullOrEmpty(dnx_options))
    {
        dnx_options = dnx_options.Trim();
    }

    Build.Log.Argument("arguments", dnx_args);
    Build.Log.Argument("options", dnx_options);
    Build.Log.Argument("path", dnx_path);
    Build.Log.Argument("wait", dnx_wait);
    Build.Log.Argument("quiet", dnx_quiet);
    Build.Log.Header();

    if (!string.IsNullOrEmpty(dnx_runtime))
    {
        if (Build.Unix)
        {
            dnx_args = string.Format(@"bash -c 'source ""{0}"" && dnvm use {1} && {2} {3} {4}'", dnvm_path, dnx_runtime, dnx_exec_cmd, dnx_args, dnx_options);
            dnx_exec_cmd = "/usr/bin/env";
        }
        else
        {
            dnx_args = string.Format(@"use {0} && {1} {2} {3}", dnx_runtime, dnx_exec_cmd, dnx_args, dnx_options);
            dnx_exec_cmd = dnvm_path;
        }

        dnx_options = string.Empty;
    }
}

exec exec_cmd='${ dnx_exec_cmd }' exec_args='${ dnx_args } ${ dnx_options }' exec_path='${ dnx_path }' exec_wait='${ dnx_wait }' exec_quiet='${ dnx_quiet }'