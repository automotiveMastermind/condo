---
layout: docs
title: exec
group: shades
---

@{/*

exec
    Executes a command at the command line.

exec_cmd=''
    Required. The command line tool to execute.

exec_args=''
    Additional arguments to send to the command line tool.

exec_path=''
    The working directory of the command line tool.

exec_wait='true'
    A value indicating whether or not to wait for exit.

exec_quiet='$(Build.Log.Quiet)'
    A value indicating whether or not to execute the command quietly.

exec_redirect='true'
    A value indicating whether or not to redirect standard output and standard error streams.

exec_retries='1'
    The number of times to retry the execution for a successful result.

exec_secure='false'
    A value indicating wehther or not to avoid printing output for public builds.

exec_output_path=''
    The path in which to write the output -- useful for creating log files.

*/}

use namespace = 'System'
use namespace = 'System.IO'

use import = 'Condo.Build'

default exec_cmd          = ''
default exec_args         = ''
default exec_path         = '${ Directory.GetCurrentDirectory() }'
default exec_wait         = '${ true }'
default exec_quiet        = '${ Build.Log.Quiet }'
default exec_redirect     = '${ true }' type='bool'
default exec_retries      = '${ 1 }' type='int'
default exec_secure       = '${ false }' type='bool'

default exec_output_path  = ''

@{
    Build.Log.Header("exec");

    if (string.IsNullOrEmpty(exec_cmd))
    {
        throw new ArgumentException("A command must be specified in order for the command to execute.", "exec_cmd");
    }

    exec_cmd = exec_cmd.Trim();

    if (!string.IsNullOrEmpty(exec_args))
    {
        exec_args = exec_args.Trim();
    }

    Build.Log.Argument("command", exec_cmd);
    Build.Log.Argument("arguments", exec_args, exec_secure);
    Build.Log.Argument("path", exec_path);
    Build.Log.Argument("wait", exec_wait);
    Build.Log.Argument("quiet", exec_quiet);
    Build.Log.Argument("redirect", exec_redirect);
    Build.Log.Argument("retries", exec_retries);
    Build.Log.Argument("secure", exec_secure);
    Build.Log.Argument("output path", exec_output_path);
    Build.Log.Header();

    // define a variable to retain the exit code
    int code;
    string result;

    // attempt to execute the command
    if(!Build.TryExecute(exec_cmd, out code, out result, exec_args, exec_path, exec_quiet, exec_wait, exec_redirect, exec_retries))
    {
        // throw a new exception
        throw new Exception(string.Format("The command {0} with args {1} failed with exit code {2}. The output was {3}.", exec_cmd, exec_args, code, exec_secure && Build.Log.Secure ? "<secured>" : result));
    }

    // determine if an output path was specified
    if (!string.IsNullOrEmpty(exec_output_path))
    {
        // write all text to the specified output path
        File.WriteAllText(exec_output_path, result);
    }

    Build.Log.Header();
}