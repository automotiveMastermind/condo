---
layout: docs
title: rimraf-download
group: shades
---

Downloads and installs rimraf if it is not already installed.

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
            <td>rimraf_path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The path that should be recursively removed.</td>
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
            <td>rimraf_download_path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>The path in which to download rimraf.</td>
        </tr>
        <tr>
            <td>base_path</td>
            <td>string</td>
            <td><code>false</code></td>
            <td>The base path in which to execute rimraf.</td>
        </tr>
    </tbody>
    </table>
</div>

## Examples

Download rimraf into specific directory

{% highlight sh %}
rimraf-download rimraf_download_path='/some/path'
{% endhighlight %}
