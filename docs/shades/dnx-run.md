---
layout: docs
title: dnx-run
group: shades
---

@{/*

dnx-run
    Executes the command for a project based on the current platform.

dnx_run_project=''
    Required. The project.json or the path containing the project.json that should be executed.

dnx_run_cmd=''
    The command that should be executed.

    NOTE: this is automatically discovered based on the current platform if not specified.

dnx_run_runtime=''
    The runtime to use when executing dnx. This argument is passed in the raw to 'dnvm use.'

dnx_run_options='' (Environment Variable: DNX_RUN_OPTIONS)
    Additional options to pass to DNX when executing the specified command.

dnx_run_quiet='Build.Log.Quiet'
    A value indicating whether or not to avoid printing output.

*/}

use namespace = 'System'
use namespace = 'System.IO'

use import = 'Condo.Build'
use import = 'Microsoft.Json'

default dnx_run_project = ''
default dnx_run_cmd     = ''
default dnx_run_runtime = ''
default dnx_run_options = '${ Build.Get("DNX_RUN_OPTIONS") }'
default dnx_run_quiet   = '${ Build.Log.Quiet }'

@{
    Build.Log.Header("dnx-run");

    var dnx_run_path        = string.Empty;
    var dnx_run_exec_path   = string.Empty;

    // determine if a project is specified
    if (string.IsNullOrEmpty(dnx_run_project))
    {
        // throw an exception
        throw new ArgumentException("dnx-run: a project must be specified.", "dnx_run_project");
    }

    dnx_run_path = File.Exists(dnx_run_project) ? dnx_run_project : Path.Combine(dnx_run_project, "project.json");

    // determine if the file still does not exist
    if (!File.Exists(dnx_run_path))
    {
        // throw an argument exception
        throw new ArgumentException("dnx-run: the specified project does not exist.", "dnx_run_project");
    }

    // determine if the command is specified
    if (string.IsNullOrEmpty(dnx_run_cmd))
    {
        // always use web for now
        dnx_run_cmd = "web";
    }

    // trim the arguments
    dnx_run_path = dnx_run_path.Trim();
    dnx_run_cmd = dnx_run_cmd.Trim();
    dnx_run_runtime = dnx_run_runtime.Trim();
    dnx_run_options = dnx_run_options.Trim();

    // get the running path
    dnx_run_exec_path = Path.GetDirectoryName(dnx_run_path);

    Build.Log.Argument("project", dnx_run_path);
    Build.Log.Argument("command", dnx_run_cmd);
    Build.Log.Argument("runtime", dnx_run_runtime);
    Build.Log.Argument("options", dnx_run_options);
    Build.Log.Argument("quiet", dnx_run_quiet);
    Build.Log.Header();

    var dnx_run_project_json = Json.Deserialize(dnx_run_path) as JsonObject;
    var dnx_run_commands_json = dnx_run_project_json.ValueAsJsonObject("commands");
    var dnx_run_exists = dnx_run_commands_json != null && dnx_run_commands_json.Keys.Contains(dnx_run_cmd);

    if (!dnx_run_exists)
    {
        Build.Log.Warn(string.Format("dnx-run: command {0} does not exist for project {1}", dnx_run_cmd, dnx_run_project));
    }
}

dnx dnx_args='${ dnx_run_cmd }' dnx_options='${ dnx_run_options }' dnx_path='${ dnx_run_exec_path }' dnx_runtime='${ dnx_run_runtime }' dnx_quiet='${ dnx_run_quiet }' dnx_wait='${ false }' if='dnx_run_exists'