---
layout: docs
title: bundle-download
group: shades
---

@{/*

bundle-download
    Downloads and installs bundler if it is not already installed.

*/}

use import = 'Condo.Build'

ruby-locate once='ruby-locate'

var bundle_install               = '${ Build.GetPath("bundle") }'
var bundle_installed             = '${ bundle_install.Global }' type='bool'

gem-install gem_install_name='bundler' if='!bundle_installed' once='bundler-install'

@{
    if (!bundle_installed)
    {
        Build.Log.Header("bundle-download");

        var bundle_version = default(string);

        bundle_installed = Build.TryExecute("bundle", out bundle_version, "--version");

        Build.Log.Argument("path", bundle_install);
        Build.Log.Argument("version", bundle_version, bundle_installed);
        Build.Log.Header();

        Build.SetPath("bundle", "bundle", true);
    }
}