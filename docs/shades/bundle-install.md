---
layout: docs
title: bundle-install
group: shades
---

Installs the packages from a Gemfile using the bundler command line utility for ruby.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `bundle-install` shade accepts the following arguments:

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
            <td>frozen</td>
            <td>boolean</td>
            <td><code>true</code></td>
            <td><strong>No</strong></td>
            <td>A value indicating whether or not to update the Gemfile.lock file after install.</td>
        </tr>
        <tr>
            <td>options</td>
            <td>string</td>
            <td><code>${env:BUNDLE_INSTALL_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to use when executing the bundle install command.</td>
        </tr>
        <tr>
            <td>path</td>
            <td>string</td>
            <td><code>${global:working_path}</code></td>
            <td>No</td>
            <td>The path in which to execute the bundle install command.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>bundle_install_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `bundle-install`:

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

### Install

{% highlight sh %}
bundle-install
{% endhighlight %}

## See Also

* [bundle]({{site.baseurl}}/shades/bundle)
* [bundle-download]({{site.baseurl}}/shades/bundle-download)