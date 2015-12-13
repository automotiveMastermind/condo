---
layout: docs
title: dnx-watch-download
group: shades
---

@{/*

dnx-watch-download
    Downloads and installs dnx-watch if it is not already installed.

*/}

use import = 'Condo.Build'

var dnx_watch_download_where_cmd    = '${ Build.Unix ? "which" : "where" }'
var dnx_watch_download_cmd          = '${ Build.Unix ? "dnx-watch" : "dnx-watch.cmd" }'
var dnx_watch_download_path         = ''
var dnx_watch_install               = '${ true }'

- dnx_watch_install = Build.TryExecute(dnx_watch_download_where_cmd, out dnx_watch_download_path, dnx_watch_download_cmd);

dnu dnu_args='commands install Microsoft.Dnx.Watcher' if='!dnx_watch_install' once='dnu-watch-install'

@{
    Build.Log.Header("dnx-watch-download");

    dnx_watch_install = Build.TryExecute(dnx_watch_download_where_cmd, out dnx_watch_download_path, dnx_watch_download_cmd);

    Build.Log.Argument("path", dnx_watch_download_path);
    Build.Log.Header();

    Build.SetPath("dnx-watch", dnx_watch_download_path, true);
}