---
layout: docs
title: dotnet-restore
group: shades
---

Executes the dotnet command line tool to restore dependencies for a project.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `dotnet-restore` shade accepts the following arguments:

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
            <td>project</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The fully-qualified path of the project to pack (`project.json`).</td>
        </tr>
        <tr>
            <td>path</td>
            <td>string</td>
            <td><code>${global:working_path}</code></td>
            <td>No</td>
            <td>The path in which to execute the dotnet pack command.</td>
        </tr>
        <tr>
            <td>output_path</td>
            <td>string</td>
            <td><code>${global:target_path}/build</code></td>
            <td>No</td>
            <td>The path in which to store the resulting package.</td>
        </tr>
        <tr>
            <td>framework</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>A semi-colon (;) delimited list of target frameworks to pack against.</td>
        </tr>
        <tr>
            <td>configuration</td>
            <td>string</td>
            <td><code>${global:configuration}</code></td>
            <td>No</td>
            <td>A semi-colon (;) delimited list of configurations to pack.</td>
        </tr>
        <tr>
            <td>options</td>
            <td>string</td>
            <td><code>${env:DOTNET_PACK_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to include when executing the dotnet command line tool for pack operations.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>dotnet_pack_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `dotnet-pack`:

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
            <td>target_path</td>
            <td>string</td>
            <td><code>${env:BUILD_BINARIESDIRECTORY}</code> -OR- <code>${global:base_path}/artifacts</code></td>
            <td>The path where build artifacts and results should be stored.</td>
        </tr>
        <tr>
            <td>configuration</td>
            <td>string</td>
            <td><code>${env:CONFIGURATION}</code> -OR- <code>Debug</code></td>
            <td>The default configuration for the build process.</td>
        </tr>
    </tbody>
    </table>
</div>

## Examples

### Build from Default Path

{% highlight sh %}
dotnet-pack
{% endhighlight %}

### Pack a Specific Project

{% highlight sh %}
dotnet-pack dotnet_pack_project='/temp/project/project.json'
{% endhighlight %}

### Pack the Debug and Release Configuration

{% highlight sh %}
dotnet-pack dotnet_pack_configuration='Debug;Release'
{% endhighlight %}

## See Also

* [dotnet-build]({{site.baseurl}}/shades/dotnet-build)
* [dotnet-clean]({{site.baseurl}}/shades/dotnet-clean)
* [dotnet-restore]({{site.baseurl}}/shades/dotnet-restore)