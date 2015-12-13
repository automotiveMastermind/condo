---
layout: docs
title: dnu-restore
group: shades
---

@{/*

dnu-restore
    Executes a dnu package manager command to restore all available packages.

dnu_restore_args=''
    The arguments to pass to the dnu command line tool (in addition to restore)

dnu_restore_options='' (Environment Variable: DNU_RESTORE_OPTIONS)
    Additional options to include when executing the dnu command line tool for the restore operation.

dnu_restore_path='$(working_path)'
    The path in which to execute the dnu command line tool.

base_path='$(CurrentDirectory)'
    The base path in which to execute the dnu command line tool.

working_path='$(base_path)'
    The working path in which to execute the dnu command line tool.

*/}

use namespace = 'System'
use namespace = 'System.IO'

use import = 'Condo.Build'

default base_path               = '${ Directory.GetCurrentDirectory() }'
default working_path            = '${ base_path }'

default dnu_restore_path        = '${ working_path }'
default dnu_restore_args        = ''
default dnu_restore_options     = '${ Build.Get("DNU_RESTORE_OPTIONS") }'

@{
    Build.Log.Header("dnu-restore");

    if (!string.IsNullOrEmpty(dnu_restore_args))
    {
        dnu_restore_args = dnu_restore_args.Trim();
    }

    if (!string.IsNullOrEmpty(dnu_restore_options))
    {
        dnu_restore_options = dnu_restore_options.Trim();
    }

    Build.Log.Argument("arguments", dnu_restore_args);
    Build.Log.Argument("options", dnu_restore_options);
    Build.Log.Argument("path", dnu_restore_path);
    Build.Log.Header();
}

dnu dnu_args='restore ${ dnu_restore_args }' dnu_options='${ dnu_restore_options }' dnu_runtime='default -r coreclr' dnu_path='${ dnu_restore_path }'