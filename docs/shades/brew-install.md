---
layout: docs
title: brew-install
group: shades
---

Installs a formula using `brew` on OS X.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %}

## Arguments

The `brew-install` shade accepts the following arguments:

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
            <td>formula</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The name of the formula to install.</td>
        </tr>
        <tr>
            <td>options</td>
            <td>string</td>
            <td><code>${env:BREW_INSTALL_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to use when installing a formula using brew-install.</td>
        </tr>
        <tr>
            <td>path</td>
            <td>string</td>
            <td><code>${global:working_path}</code></td>
            <td>No</td>
            <td>The path in which to execute brew-install.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>brew_install_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `brew-install`:

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

### Install Formula

{% highlight sh %}
brew-install brew_install_formula='graphicsmagick'
{% endhighlight %}

## See Also

* [brew]({{site.baseurl}}/shades/brew)
* [brew-download]({{site.baseurl}}/shades/brew-download)