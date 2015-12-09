---
layout: docs
title: Ruby
group: shades
---

Executes the ruby command line tool.

## Contents

* Will be replaced with the table of contents
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
            <td>ruby_args</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The arguments to pass to the ruby interpreter.</td>
        </tr>
        <tr>
            <td>ruby_options</td>
            <td>string</td>
            <td><code>-e</code></td>
            <td>No</td>
            <td>The options to use with the Ruby interpreter.</td>
        </tr>
        <tr>
            <td>ruby_path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The path in which to execute the Ruby interpreter.</td>
        </tr>
        <tr>
            <td>ruby_wait</td>
            <td>boolean</td>
            <td><code>true</code></td>
            <td>No</td>
            <td>A value indicating whether or not to wait for exit.</td>
        </tr>
        <tr>
            <td>ruby_quiet</td>
            <td>string</td>
            <td><code>$(Build.Log.Quiet)</code></td>
            <td>No</td>
            <td>A value indicating whether or not to wait for exit.</td>
        </tr>
    </tbody>
    </table>
</div>

## Global Arguments

The following global arguments are used by Ruby:

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

Execute Ruby with options and arguments

{% highlight sh %}
ruby ruby_options='-e' ruby_args='puts "hello world"'
{% endhighlight %}