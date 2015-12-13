---
layout: docs
title: git-config
group: shades
---

@{/*

git-config
    Sets the username and email address for use with git commands that interact with remotes.

git_config_user='' (Environment Variable: GIT_AUTHOR_NAME or GIT_COMMITTER_NAME or USER)
    Required. The name of the user used to interact with remotes.

git_config_email='' (Environment Variable: GIT_AUTHOR_EMAIL or GIT_COMMITTER_EMAIL or EMAIL)
    Required. The email of the user used to interact with remotes.

git_config_quiet='$(Build.Log.Quiet)'
    A value indicating whether or not to avoid printing output.

git_config_path='$(working_path)'
    The path in which to execute git.

base_path='$(CurrentDirectory)'
    The base path in which to execute git.

working_path='$(base_path)'
    The working path in which to execute git.

*/}

use namespace = 'System'

use import = 'Condo.Build'

default base_path           = '${ Directory.GetCurrentDirectory() }'
default working_path        = '${ base_path }'

default git_config_path     = '${ working_path }'
default git_config_user     = '${ Build.Get("GIT_USER_NAME") }'
default git_config_email    = '${ Build.Get("GIT_USER_EMAIL") }'
default git_config_quiet    = '${ Build.Log.Quiet }'

@{
    Build.Log.Header("git-config");

    var git_version         = string.Empty;
    var git_installed       = Build.TryExecute("git", out git_version, "--version");

    if (!git_installed)
    {
        throw new Exception("git: git must be manually installed in order to use the git cmd.");
    }

    if (string.IsNullOrEmpty(git_config_user))
    {
        git_config_user = Build.Get("GIT_AUTHOR_NAME");
    }

    if (string.IsNullOrEmpty(git_config_user))
    {
        git_config_user = Build.Get("GIT_COMMITTER_NAME");
    }

    if (string.IsNullOrEmpty(git_config_user))
    {
        git_config_user = Build.Get("USER");
    }

    git_config_user.Trim();

    if (string.IsNullOrEmpty(git_config_user))
    {
        throw new ArgumentException("git-config: a user is required.", "git_config_user");
    }

    if (string.IsNullOrEmpty(git_config_email))
    {
        git_config_email = Build.Get("GIT_AUTHOR_EMAIL");
    }

    if (string.IsNullOrEmpty(git_config_email))
    {
        git_config_email = Build.Get("GIT_COMMITTER_EMAIL");
    }

    if (string.IsNullOrEmpty(git_config_email))
    {
        git_config_email = Build.Get("EMAIL");
    }

    git_config_email.Trim();

    if (string.IsNullOrEmpty(git_config_email))
    {
        throw new ArgumentException("git-config: an email address is required.", "git_config_email");
    }

    Build.Log.Argument("version", git_version);
    Build.Log.Argument("user", git_config_user, Build.Log.Secure);
    Build.Log.Argument("email", git_config_email, Build.Log.Secure);
    Build.Log.Header();
}

exec exec_cmd='git' exec_args='config user.name "${ git_config_user }"' exec_path='${ git_config_path }' exec_quiet='${ git_config_quiet }' exec_secure='${ Build.Log.Secure }'
exec exec_cmd='git' exec_args='config user.email "${ git_config_email }"' exec_path='${ git_config_path }' exec_quiet='${ git_config_quiet }' exec_secure='${ Build.Log.Secure }'