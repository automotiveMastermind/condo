---
layout: docs
title: git
group: shades
---

@{/*

git
    Executes a git command at the command line.

git_args=''
    Required. The arguments for the git command.

git_options='' (Environment Variable: GIT_OPTIONS)
    Additional options to use when executing the git command.

git_path='$(working_path)'
    The path in which to execute git.

base_path='$(CurrentDirectory)'
    The base path in which to execute git.

working_path='$(base_path)'
    The working path in which to execute git.

git_wait='true'
    A value indicating whether or not to wait for exit.

git_quiet='$(Build.Log.Quiet)'
    A value indicating whether or not to avoid printing output.

*/}

use namespace = 'System'
use namespace = 'System.IO'

use import = 'Condo.Build'

default base_path       = '${ Directory.GetCurrentDirectory() }'
default working_path    = '${ base_path }'

default git_args        = ''
default git_path        = '${ working_path }'
default git_options     = '${ Build.Get("GIT_OPTIONS") }'
default git_wait        = '${ true }'
default git_quiet       = '${ Build.Log.Quiet }'

git-config once='git-config'

@{
    Build.Log.Header("git");

    if (string.IsNullOrEmpty(git_args))
    {
        throw new ArgumentException("git: arguments must be specified.", "git_args");
    }

    git_args = git_args.Trim();

    if (!string.IsNullOrEmpty(git_options))
    {
        git_options = git_options.Trim();
    }

    Build.Log.Argument("arguments", git_args, Build.Log.Secure);
    Build.Log.Argument("options", git_options);
    Build.Log.Argument("path", git_path);
    Build.Log.Argument("wait", git_wait);
    Build.Log.Argument("quiet", git_quiet);
    Build.Log.Header();
}

exec exec_cmd='git' exec_args='${ git_options } ${ git_args }' exec_path='${ git_path }' exec_wait='${ git_wait }' exec_quiet='${ git_quiet }' exec_redirect='${ false }'