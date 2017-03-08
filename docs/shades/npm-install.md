---
layout: docs
title: npm-install
group: shades
---

@{/*

npm-install
    Installs a module using the node package manager (npm).

npm_install_id=''
    The identifier of the module to install with npm. If no id is specified, the package.json file at the npm_install_path will be used.

npm_install_prefix='$(base_path)'
    The prefix path to use when installing the NPM module.

npm_install_options='' (Environment Variable: NPM_INSTALL_OPTIONS)
    Additional options to use when executing the npm install command.

npm_install_path='$(working_path)'
    The path in which to execute the npm install command.

working_path='$(base_path)'
    The working path in which to execute the npm install command.

base_path='$(CurrentDirectory)'
    The base path in which to execute the npm install command.

*/}

use namespace = 'System'

use import = 'Condo.Build'

default base_path           = '${ Directory.GetCurrentDirectory() }'
default working_path        = '${ base_path }'

default npm_install_id      = ''
default npm_install_options = '${ Build.Get("NPM_INSTALL_OPTIONS") }'
default npm_install_path    = '${ working_path }'
default npm_install_prefix  = '${ working_path }'

@{
    Build.Log.Header("npm-install");

    // trim variables
    npm_install_id = npm_install_id.Trim();
    npm_install_options = npm_install_options.Trim();

    Build.Log.Argument("module id", npm_install_id);
    Build.Log.Argument("prefix", npm_install_prefix);
    Build.Log.Argument("options", npm_install_options);
    Build.Log.Argument("path", npm_install_path);
    Build.Log.Header();
}

npm npm_args='install ${ npm_install_id }' npm_options='--prefix "${ npm_install_prefix }" ${ npm_install_options }' npm_path='${ npm_install_path }' if='!string.IsNullOrEmpty(npm_install_id)'
npm npm_args='install' npm_options='${ npm_install_options }' npm_path='${ npm_install_path }' if='string.IsNullOrEmpty(npm_install_id)'