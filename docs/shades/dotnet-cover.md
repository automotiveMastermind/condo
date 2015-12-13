---
layout: docs
title: dotnet-cover
group: shades
---

Analyzes .NET 5 executions for code coverage results.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-windows fa-3x %}

## Arguments

The `dotnet-cover` shade accepts the following arguments:

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
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The target process for which to collect code coverage results.</td>
        </tr>
        <tr>
            <td>pdb_path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The path which contains the PDBs for the assemblies for which code coverage results should be processed.</td>
        </tr>
        <tr>
            <td>runtime</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The version of the runtime to use when executing the test process.</td>
        </tr>
        <tr>
            <td>args</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The arguments to pass to the code coverage target.</td>
        </tr>
        <tr>
            <td>options</td>
            <td>string</td>
            <td><code>-nodefaultfilters -hideskipped:All -threshold:1</code></td>
            <td>No</td>
            <td>Additional options to pass to the code coverage process.</td>
        </tr>
        <tr>
            <td>exclude_files</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>A semi-colon (;) delimited list of file name globbing patterns that will be excluded from code coverage results.</td>
        </tr>
        <tr>
            <td>exclude_attributes</td>
            <td>string</td>
            <td><code>*.ExcludeFromCodeCoverage*</code></td>
            <td>No</td>
            <td>A semi-colon (;) delimited list of attribute name globbing patterns that will be excluded from code coverage results.</td>
        </tr>
        <tr>
            <td>filter</td>
            <td>string</td>
            <td><code>+[*]* -[Xunit]* -[Xunit.*]* -[xunit]* -[xunit.*]*</code></td>
            <td>No</td>
            <td>A semi-colon (;) delimited list of file name globbing patterns that will be excluded from code coverage results.</td>
        </tr>
        <tr>
            <td>register</td>
            <td>string</td>
            <td><code>user</code></td>
            <td>No</td>
            <td>Determines the level of registration of the code coverage profiler..</td>
        </tr>
        <tr>
            <td>show_unvisited</td>
            <td>boolean</td>
            <td><code>false</code></td>
            <td>No</td>
            <td>A value indicating whether or not to list unvisted methods and classes once the coverage analysis is complete.</td>
        </tr>
        <tr>
            <td>skip_autoprops</td>
            <td>boolean</td>
            <td><code>true</code></td>
            <td>No</td>
            <td>A value indicating whether or not to skip code coverage of automatic properties.</td>
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
            <td>A value indicating whether or not to suppress standard output when analyzing code coverage.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>dotnet_cover_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The following global arguments are used by `dotnet-cover`:

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

### Coverage with CoreCLR

{% highlight sh %}
dotnet-cover dotnet_cover_target='dnx' dotnet_cover_runtime='default -r coreclr' dotnet_cover_args='test -xml "artifacts/test/myproject.test-dnxcore50.xml"'
{% endhighlight %}

## See Also

* [cover-report]({{site.baseurl}}/shades/cover-report)