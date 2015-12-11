---
layout: docs
title: nuget-push
group: shades
---

Executes a nuget pushage manager command to push all available pushages.

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
            <td>nuget_push_args</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The path to the nuget package that should be pushed to the feed.</td>
        </tr>
        <tr>
            <td>nuget_push_feed</td>
            <td>string</td>
            <td><code>${env:NUGET_PUSH_FEED}</code></td>
            <td>No</td>
            <td>The feed to which nuget should push packages.</td>
        </tr>
        <tr>
            <td>nuget_push_apikey</td>
            <td>string</td>
            <td><code>$(NUGET_PUSH_APIKEY)</code></td>
            <td>No</td>
            <td>The API key to use when pushing packages to the nuget feed.</td>
        </tr>
        <tr>
            <td>nuget_push_options</td>
            <td>string</td>
            <td><code>$(NUGET_PUSH_OPTIONS)</code></td>
            <td>No</td>
            <td>Additional options to include when executing the nuget command line tool for the push operation.</td>
        </tr>
        <tr>
            <td>nuget_config_path</td>
            <td>string</td>
            <td><code>$(nuget_download_path)/nuget.config</code></td>
            <td>No</td>
            <td>The path to the nuget configuration file to use when executing nuget commands.</td>
        </tr>
        <tr>
            <td>nuget_push_path</td>
            <td>string</td>
            <td><code>$(working_path)</code></td>
            <td>No</td>
            <td>The path in which to execute the nuget command line tool.</td>
        </tr>
        <tr>
            <td>nuget_download_path</td>
            <td>string</td>
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
    </tbody>
    </table>
</div>

## Examples

Push package which is located in a specific directory

{% highlight sh %}
nuget-push nuget_push_args='/some/path'
{% endhighlight %}

Push package to server with a specific API key

{% highlight sh %}
nuget-push nuget_push_args='/some/path' nuget_push_apikey='2k34hkj23h4k2h34kh234'
{% endhighlight %}
