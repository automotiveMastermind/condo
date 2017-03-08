---
layout: docs
title: bower-install
group: shades
---

Installs a package with the specified id using the bower package manager command.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `bower-install` shade accepts the following arguments:

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
            <td>id</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The identifier of the package to install using bower.</td>
        </tr>
        <tr>
            <td>options</td>
            <td>string</td>
            <td><code>${env:BOWER_INSTALL_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to use when executing the bower command.</td>
        </tr>
        <tr>
            <td>path</td>
            <td>string</td>
            <td><code>${global:working_path}</code></td>
            <td>No</td>
            <td>The path in which to execute the bower install command.</td>
        </tr>
        <tr>
            <td>quiet</td>
            <td>boolean</td>
            <td><code>${global:quiet}</code></td>
            <td>No</td>
            <td>A value indicating whether or not to suppress standard output when executing the bower-install command.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>bower_install_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `bower-install`:

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
            <td>The base path in which Condo was executed.</td>
        </tr>
        <tr>
            <td>working_path</td>
            <td>string</td>
            <td><code>${global:base_path}</code></td>
            <td>The working path in which Condo should execute shell commands.</td>
        </tr>
        <tr>
            <td>quiet</td>
            <td>boolean</td>
            <td><code>false</code></td>
            <td>A value indicating whether or not to suppress output when executing Condo.</td>
        </tr>
    </tbody>
    </table>
</div>

## Examples

### Install

{% highlight sh %}
bower install
{% endhighlight %}

### Install with Options

{% highlight sh %}
bower-install bower_install_options='--save-dev'
{% endhighlight %}

This would not be a likely scenario for a repeatable process, but its still possible.

## See Also

* [bower](/{{site.baseurl}}/shades/bower)
* [bower-download]({{site.baseurl}}/shades/bower-download)