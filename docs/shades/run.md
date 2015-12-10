---
layout: docs
title: run
group: shades
---

Executes the run command line tool.

## Contents

* Will be replaced with the ToC, excluding the "Contents" header
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The following optional arguments are available within bower.

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
            <td>run_args</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The arguments for the run command.</td>
        </tr>
        <tr>
            <td>run_options</td>
            <td>string</td>
            <td><code>${env:RUN_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to use when executing the run command.</td>
        </tr>
        <tr>
            <td>run_path</td>
            <td>string</td>
            <td><code>${global:working_path}</code></td>
            <td>No</td>
            <td>The path in which to execute run.</td>
        </tr>
        <tr>
            <td>run_wait</td>
            <td>boolean</td>
            <td><code>true</code></td>
            <td>No</td>
            <td>A value indicating whether or not to wait for exit.</td>
        </tr>
        <tr>
            <td>run_quiet</td>
            <td>boolean</td>
            <td><code>${global:quiet}</code></td>
            <td>No</td>
            <td>A value indicating whether or not to avoid printing output.</td>
        </tr>
        <tr>
            <td>run_secure</td>
            <td>boolean</td>
            <td><code>false</code></td>
            <td>No</td>
            <td>A value indicating whether or not to avoid printing secure information for public builds.</td>
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
        <tr>
            <td>quiet</td>
            <td>boolean</td>
            <td><code>false</code></td>
            <td>A value indicating whether or not to suppress output when executing condo.</td>
        </tr>
    </tbody>
    </table>
</div>

## Examples

Condo can execute any managed executable, regardless of the platform. On OS X and Linux, the arguments are
passed to the mono bootstrapper. We hope to remove this dependency in the future, but at the present time,
condo itself relies on mono, so this dependency would always be available.

{% highlight sh %}
run run_args='nuget.exe'
{% endhighlight %}

### Run with Options

{% highlight sh %}
run run_args='nuget.exe' run_options='restore'
{% endhighlight %}

### Run in a Path

{% highlight sh %}
run run_args='example_command.exe' run_path='./myfolder'
{% endhighlight %}

## See Also

* [exec](/shades/exec)