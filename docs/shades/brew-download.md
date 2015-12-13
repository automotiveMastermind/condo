---
layout: docs
title: brew-download
group: shades
---

Downloads brew on OS X if it is not already available.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %}

## Arguments

The `brew-download` shade does not accept any arguments.

## Global Arguments

The following global arguments are used by `brew-download`:

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
    </tbody>
    </table>
</div>

## Examples

Condo can download and install `brew` if it is not already available on the system.

{% highlight sh %}
brew-download
{% endhighlight %}

## See Also

* [brew]({{site.baseurl}}/shades/brew)
* [brew-install]({{site.baseurl}}/shades/brew-install)