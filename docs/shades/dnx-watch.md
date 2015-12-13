---
layout: docs
title: dnx-watch
group: shades
---

@{/*

dnx-watch
    Executes the dnx command for a project based on the current platform and watches for changes.

dnx_watch_project=''
    Required. The project.json or the path containing the project.json that should be executed.

dnx_watch_cmd=''
    The command that should be executed.

    NOTE: this is automatically discovered based on the current platform if not specified.

dnx_watch_runtime='default -r coreclr'
    The runtime to use when executing dnx. This argument is passed in the raw to 'dnvm use.'

dnx_watch_options='' (Environment Variable: DNX_WATCH_OPTIONS)
    Additional options to pass to DNX when executing the specified command.

dnx_watch_quiet='Build.Log.Quiet'
    A value indicating whether or not to avoid printing output.

*/}

use namespace = 'System'

use import = 'Condo.Build'

dnx-watch-download once='dnx-watch-download'

default dnx_watch_project     = ''
default dnx_watch_cmd         = ''
default dnx_watch_runtime     = 'default -r coreclr'
default dnx_watch_options     = '${ Build.Get("DNX_WATCH_OPTIONS") }'
default dnx_watch_quiet       = '${ Build.Log.Quiet }'

@{
    Build.Log.Header("dnx-watch");

    var dnx_watch_args        = dnx_watch_project;
    var dnx_watch_dnvm_path   = Build.GetPath("dnvm").Path;
    var dnx_watch_path        = Build.GetPath("dnx-watch").Path;
    var dnx_watch_exec_path   = string.Empty;

    // determine if a project is specified
    if (string.IsNullOrEmpty(dnx_watch_args))
    {
        // throw an exception
        throw new ArgumentException("dnx-watch: a project must be specified.", "dnx_watch_project");
    }

    dnx_watch_args = File.Exists(dnx_watch_args) ? dnx_watch_args : Path.Combine(dnx_watch_args, "project.json");

    // determine if the file still does not exist
    if (!File.Exists(dnx_watch_args))
    {
        // throw an argument exception
        throw new ArgumentException("dnx-watch: the specified project does not exist.", "dnx_watch_project");
    }

    // determine if the command is specified
    if (string.IsNullOrEmpty(dnx_watch_cmd))
    {
        // always use web for now
        dnx_watch_cmd = "web";
    }

    // trim the arguments
    dnx_watch_args = dnx_watch_args.Trim();
    dnx_watch_cmd = dnx_watch_cmd.Trim();
    dnx_watch_runtime = dnx_watch_runtime.Trim();
    dnx_watch_options = dnx_watch_options.Trim();

    // get the exec path
    dnx_watch_exec_path = Path.GetDirectoryName(dnx_watch_args);

    Build.Log.Argument("project", dnx_watch_args);
    Build.Log.Argument("command", dnx_watch_cmd);
    Build.Log.Argument("runtime", dnx_watch_runtime);
    Build.Log.Argument("options", dnx_watch_options);
    Build.Log.Argument("quiet", dnx_watch_quiet);
    Build.Log.Header();

    var js = new JavaScriptSerializer();

    var dnx_watch_text = File.ReadAllText(dnx_watch_args);
    var dnx_watch_json = js.DeserializeObject(dnx_watch_text) as Dictionary<string, object>;

    object dnx_watch_cmds_obj;

    var dnx_watch_cmds = dnx_watch_json.TryGetValue("commands", out dnx_watch_cmds_obj)
        ? dnx_watch_cmds_obj as Dictionary<string, object>
        : new Dictionary<string, object>();

    object dnx_watch_cmd_obj;

    var dnx_watch_cmd_exists = dnx_watch_cmds.TryGetValue(dnx_watch_cmd, out dnx_watch_cmd_obj);

    if (dnx_watch_cmd_exists)
    {
        // define the arguments
        dnx_watch_args = string.Format(@"--dnx-args {0} {1}", dnx_watch_cmd, dnx_watch_options).Trim();

        if (!string.IsNullOrEmpty(dnx_watch_runtime))
        {
            if (Build.Unix)
            {
                dnx_watch_args = string.Format(@"bash -c 'source ""{0}"" && dnvm use {1} && {2} {3}'", dnx_watch_dnvm_path, dnx_watch_runtime, dnx_watch_path, dnx_watch_args);
                dnx_watch_path = "/usr/bin/env";
            }
            else
            {
                dnx_watch_args = string.Format(@"use {0} && {1} {2}", dnx_watch_runtime, dnx_watch_path, dnx_watch_args);
                dnx_watch_path = dnx_watch_dnvm_path;
            }
        }
    }

    if (!dnx_watch_cmd_exists)
    {
        Build.Log.Warn(string.Format("dnx-watch: command {0} does not exist for project {1}", dnx_watch_cmd, dnx_watch_project));
    }
}

exec exec_cmd='${ dnx_watch_path }' exec_args='${ dnx_watch_args }' exec_path='${ dnx_watch_exec_path }' exec_wait='${ false }' exec_quiet='${ dnx_watch_quiet }' if='dnx_watch_cmd_exists'