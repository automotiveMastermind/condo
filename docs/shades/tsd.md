---
layout: docs
title: tsd
group: shades
---

Executes a DefinitlyTyped typings manager.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `tsd` shade accepts the following arguments:

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
            <td>args</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The arguments to pass to the tsd command line tool.</td>
        </tr>
        <tr>
            <td>options</td>
            <td>string</td>
            <td><code>-e</code></td>
            <td>No</td>
            <td>Additional options to use when executing the tsd command.</td>
        </tr>
        <tr>
            <td>path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The path in which to execute tsd.</td>
        </tr>
        <tr>
            <td>wait</td>
            <td>boolean</td>
            <td><code>true</code></td>
            <td>No</td>
            <td>A value indicating whether or not to wait for exit.</td>
        </tr>
        <tr>
            <td>quiet</td>
            <td>string</td>
            <td><code>$(Build.Log.Quiet)</code></td>
            <td>No</td>
            <td>A value indicating whether or not to avoid printing output.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>tsd_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `tsd`:

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

### Install with Options

{% highlight sh %}
tsd tsd_args='install mocha' tsd_options='--save'
{% endhighlight %}

### Reinstall with Options
{% highlight sh %}
tsd tsd_args='reinstall' tsd_options='--clean --overwrite --save'
{% endhighlight %}