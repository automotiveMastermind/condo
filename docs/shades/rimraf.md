---
layout: docs
title: rimraf
group: shades
---

Executes rimraf against the specified path.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `rimraf` shade accepts the following arguments:

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
            <td>path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The path that should be recursively removed.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>rimraf_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `rimraf`:

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

### Recursively Delete Path

{% highlight sh %}
rimraf rimraf_path='/some/path'
{% endhighlight %}
