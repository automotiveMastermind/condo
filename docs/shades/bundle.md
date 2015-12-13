---
layout: docs
title: bundle
group: shades
---

Executes the bundler command line utility.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `bundle` shade accepts the following arguments:

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
            <td>bundle_args</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The arguments to pass to the bundler command line tool.</td>
        </tr>
        <tr>
            <td>bundle_options</td>
            <td>string</td>
            <td><code>${env:BUNDLE_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to use when executing the bundler command.</td>
        </tr>
        <tr>
            <td>bundle_path</td>
            <td>string</td>
            <td><code>${global:working_path}</code></td>
            <td>No</td>
            <td>The path in which to execute bundler.</td>
        </tr>
        <tr>
            <td>bundle_wait</td>
            <td>boolean</td>
            <td><code>true</code></td>
            <td>No</td>
            <td>A value indicating whether or not to wait for the bundler command to exit.</td>
        </tr>
        <tr>
            <td>bundle_quiet</td>
            <td>boolean</td>
            <td><code>${global:quiet}</code></td>
            <td>No</td>
            <td>A value indicating whether or not to suppress standard output when executing the bundler command.</td>
        </tr>
    </tbody>
    </table>
</div>

## Global Arguments

The following global arguments are used by `bundle`:

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

### Bundler

{% highlight sh %}
bundle bundle_args='install'
{% endhighlight %}

Note: Use the `bundle-install` shade instead of calling `bundle` directly as illustrated in this example.
This shade exists primarily to support more specialized shades.

## See Also

* [bundle-download]({{site.baseurl}}/shades/bundle-download)
* [bundle-install]({{site.baseurl}}/shades/bundle-install)