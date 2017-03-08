---
layout: docs
title: dotnet-cover-report
group: shades
---

Generates a human-readable report for code coverage results.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-windows fa-3x %}

## Arguments

The `dotnet-cover-report` shade accepts the following arguments:

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
            <td>target</td>
            <td>string</td>
            <td><code>${global:target_test_path}/*-coverage.xml</code></td>
            <td>No</td>
            <td>The path containing the code coverage results that should be parsed.</td>
        </tr>
        <tr>
            <td>output_path</td>
            <td>string</td>
            <td><code>${global:target_test_path}</code></td>
            <td>No</td>
            <td>The path where the report will be emitted.</td>
        </tr>
        <tr>
            <td>type</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The type of the reports to generate.</td>
        </tr>
        <tr>
            <td>src_path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The path(s) containing source files for reference.</td>
        </tr>
        <tr>
            <td>history_path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The path where historical coverage reports should be retained.</td>
        </tr>
        <tr>
            <td>assembly_filter</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The filter(s) used to remove specific assemblies from the report.</td>
        </tr>
        <tr>
            <td>class_filter</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The filter(s) used to remove specific classes from the report.</td>
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
            <td>A value indicating whether or not to suppress standard output when generate code coverage report.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>dotnet_cover_report_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `dotnet-cover-report`:

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
            <td>target_test_path</td>
            <td>string</td>
            <td><code>${env:COMMON_TESTRESULTSDIRECTORY}</code> -OR- <code>${global:target_path}/test</code></td>
            <td>The path where test results should be stored.</td>
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

### Generate Report from Default Path

{% highlight sh %}
dotnet-cover-report
{% endhighlight %}

### Generate Report from Path

{% highlight sh %}
dotnet-cover-report dotnet_cover_report_target='/temp/*-coverage.xml'
{% endhighlight %}

## See Also

* [dotnet-cover]({{site.baseurl}}/shades/dotnet-cover)
* [dotnet-test]({{site.baseurl}}/shades/dotnet-test)