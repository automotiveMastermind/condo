---
layout: docs
title: dnu
group: shades
---

[DEPRECATED] Executes the .NET utility command line tool.

Note: The dnu tool will be removed in the RC2 release of .NET 5. As such, directly
executing this shade is now deprecated to avoid build system issues when RC2 is publically available.
For the time being, the shade must remain as a dependency to support existing RC1 builds.

Please use the `dotnet-*` shades to perform operations using this utility.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `dnu` shade accepts the following arguments:

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
            <td>No</td>
            <td>The arguments to pass to the dnu command line tool.</td>
        </tr>
        <tr>
            <td>options</td>
            <td>string</td>
            <td><code>${env:DNU_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to include when executuing the dnu command line tool.</td>
        </tr>
        <tr>
            <td>path</td>
            <td>string</td>
            <td><code>${global:working_path}</code></td>
            <td>No</td>
            <td>The path in which to execute the dnu command line tool.</td>
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
            <td>boolean</td>
            <td><code>${global:quiet}</code></td>
            <td>No</td>
            <td>A value indicating whether or not to avoid printing output.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>dnu_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `dnu`:

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

### Build from Default Path

{% highlight sh %}
dnu dnu_args='build'
{% endhighlight %}

### Restore from Path

{% highlight sh %}
dnu dnu_args='restore' dnu_path='/temp/custom/'
{% endhighlight %}

## See Also

* [dotnet-build]({{site.baseurl}}/shades/dotnet-build)
* [dotnet-restore]({{site.baseurl}}/shades/dotnet-restore)
* [dotnet-pack]({{site.baseurl}}/shades/dotnet-pack)