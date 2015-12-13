---
layout: docs
title: bower-download
group: shades
slug: bower-download
---

Downloads and installs bower if it is not already installed.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `bower-download` shade accepts the following arguments:

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
            <td>bower_download_path</td>
            <td>string</td>
            <td><code>${global:base_path}</code></td>
            <td>No</td>
            <td>The path in which to download bower.</td>
        </tr>
    </tbody>
    </table>
</div>

## Global Arguments

The following global arguments are used by `bower-download`:

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
    </tbody>
    </table>
</div>

## Examples

### Download

{% highlight sh %}
bower-download
{% endhighlight %}

## See Also

* [bower]({{site.baseurl}}/shades/bower)
* [bower-install]({{site.baseurl}}/shades/bower-download)
