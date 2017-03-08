---
layout: docs
title: mocha
group: shades
---

@{/*

mocha
    Executes a mocha package manager command.

mocha_args=''
    The arguments to pass to the mocha command line tool.

mocha_options='' (Environment Variable: MOCHA_OPTIONS)
    Additional options to use when executing the mocha command.

mocha_reporter='' (Environment Variable: MOCHA_REPORTER)
    The reporter to use to execute the unit tests.

mocha_reporter_options='' (Environment Variable: MOCHA_REPORTER_OPTIONS)
    Additional options to use with the included reporter.

mocha_path='$(base_path)'
    The path in which to execute mocha.

base_path='$(CurrentDirectory)'
    The base path in which to execute mocha.

working_path='$(base_path)'
    The working path in which to execute mocha.

*/}

use namespace = 'System'
use namespace = 'System.IO'

use import = 'Condo.Build'

default base_path               = '${ Directory.GetCurrentDirectory() }'
default working_path            = '${ base_path }'
default target_path             = '${ Build.Get("BUILD_BINARIESDIRECTORY", Path.Combine(base_path, "artifacts")) }'
default target_test_path        = '${ Build.Get("COMMON_TESTRESULTSDIRECTORY", Path.Combine(target_path, "test")) }'

default mocha_args              = ''
default mocha_options           = '${ Build.Get("MOCHA_OPTIONS") }'
default mocha_reporter          = '${ Build.Get("MOCHA_REPORTER") }'
default mocha_reporter_options  = '${ Build.Get("MOCHA_REPORTER_OPTIONS") }'

default mocha_path              = '${ working_path }'

mocha-download once='mocha-download'

@{
    Build.Log.Header("mocha");

    var mocha_cmd               = Build.GetPath("mocha");

    // trim the arguments
    mocha_args = mocha_args.Trim();
    mocha_options = mocha_options.Trim();

    Build.Log.Argument("cli", mocha_cmd.Path);
    Build.Log.Argument("arguments", mocha_args);
    Build.Log.Argument("options", mocha_options);
    Build.Log.Argument("reporter", mocha_reporter);
    Build.Log.Argument("reporter opt", mocha_reporter_options);
    Build.Log.Argument("path", mocha_path);
    Build.Log.Header();

    if (!string.IsNullOrEmpty(mocha_reporter))
    {
        mocha_options = mocha_options + " --reporter " + mocha_reporter;
    }

    if (!string.IsNullOrEmpty(mocha_reporter_options))
    {

        // set the output path
        mocha_options = string.Format(@"{0} --reporter-options {1}", mocha_options, mocha_reporter_options);
    }

    mocha_options = mocha_options.Trim();
}

exec exec_cmd='${ mocha_cmd.Path }' exec_args='${ mocha_args } ${ mocha_options }' exec_path='${ mocha_path }' exec_redirect='${ false }' if='mocha_cmd.Global'
node node_args='"${ mocha_cmd.Path }" ${ mocha_args } ${ mocha_options }' node_path='${ mocha_path }' if='!mocha_cmd.Global'