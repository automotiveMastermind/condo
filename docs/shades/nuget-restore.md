---
layout: docs
title: nuget-restore
group: shades
---

Executes a nuget package manager command to restore all available packages.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The following arguments are available within bower.

<div class="table-responsive">
    <table class="table table-bordered table-striped">
    <thead>
        <tr>
            <th style="width:100px;">Name</th>
            <th style="width:50px;">Type</th>
            <th style="width:50px;">Default</th>
            <th style="width:25px;">Required</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>nuget_restore_args</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>No</strong></td>
            <td>The arguments to pass to the nuget command line tool (in addition to restore).</td>
        </tr>
        <tr>
            <td>nuget_restore_options</td>
            <td>string</td>
            <td><code>${env:NUGET_RESTORE_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to include when executing the nuget command line tool for the restore operation.</td>
        </tr>
        <tr>
            <td>nuget_restore_path</td>
            <td>string</td>
            <td><code>$(working_path)</code></td>
            <td>No</td>
            <td>The path in which to execute the nuget command line tool.</td>
        </tr>
        <tr>
            <td>nuget_config_path</td>
            <td>string</td>
            <td><code>$(nuget_download_path)/nuget.config</code></td>
            <td>No</td>
            <td>The path to the nuget configuration file to use when executing nuget commands.</td>
        </tr>
        <tr>
            <td>nuget_download_path</td>
            <td>boolean</td>
            <td><code>$(base_path)/.nuget</code></td>
            <td>No</td>
            <td>The path in which to install nuget.</td>
        </tr>
    </tbody>
    </table>
</div>

## Global Arguments

The following global arguments are used by bower:

<div class="table-responsive">
    <table class="table table-bordered table-striped">
    <thead>
        <tr>
            <th style="width:100px;">Name</th>
            <th style="width:50px;">Type</th>
            <th style="width:50px;">Default</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>base_path</td>
            <td>string</td>
            <td><code>$PWD</code></td>
            <td>The base path in which condo was executed.</td>
        </tr>
        <tr>
            <td>working_path</td>
            <td>string</td>
            <td><code>${global:base_path}</code></td>
            <td>The working path in which condo should execute shell commands.</td>
        </tr>
        <tr>
            <td>quiet</td>
            <td>boolean</td>
            <td><code>false</code></td>
            <td>A value indicating whether or not to suppress output when executing condo.</td>
        </tr>
    </tbody>
    </table>
</div>

## Examples

Execute packages.config file in specified folder to restore packages:

{% highlight sh %}
nuget-restore nuget_restore_path='/some/path'
{% endhighlight %}

Execute packages.config file in specified folder to restore packages while bypassing cache:

{% highlight sh %}
nuget-restore nuget_restore_path='/some/path' nuget_restore_args='-NoCache'
{% endhighlight %}

