---
layout: docs
title: dnvm-locate
group: shades
---

@{/*

dnvm-locate
    Locates the dnvm script and adds it the build paths.

*/}

use namespace = 'System'
use namespace = 'System.IO'

@{
    var dnvm_locate_where_cmd = Build.Unix ? "which" : "where";
    var dnvm_locate_cmd = Build.Unix ? "dnvm.sh" : "dnvm.cmd";
    var dnvm_locate_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dnx", "dnvm", dnvm_locate_cmd);
    var dnvm_locate_found = false;

    Build.Log.Header("dnvm-locate");

    dnvm_locate_found = Build.TryExecute(dnvm_locate_where_cmd, out dnvm_locate_cmd, dnvm_locate_cmd);

    if (!dnvm_locate_found)
    {
        dnvm_locate_found = Build.TryExecute(dnvm_locate_where_cmd, out dnvm_locate_cmd, "dnvm");
    }

    if (!dnvm_locate_found)
    {
        dnvm_locate_cmd = dnvm_locate_path;
        dnvm_locate_found = File.Exists(dnvm_locate_cmd);
    }

    if (!dnvm_locate_found)
    {
        throw new NotSupportedException("dnvm-locate: could not find an instance of dnvm -- the build process cannot continue.");
    }

    Build.SetPath("dnvm", dnvm_locate_cmd, true);

    Build.Log.Argument("path", dnvm_locate_cmd);
    Build.Log.Header();
}