---
layout: docs
title: copy
group: shades
---

Copies one or more files from the source path to the destination path.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `copy` shade accepts the following arguments:

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
            <td>src_path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The path from which to copy files.</td>
        </tr>
        <tr>
            <td>dst_path</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The path to which to copy files.</td>
        </tr>
        <tr>
            <td>include</td>
            <td>string</td>
            <td><code>**/*.*</code></td>
            <td>No</td>
            <td>The filter used to include files within the specified source path.</td>
        </tr>
        <tr>
            <td>exclude</td>
            <td>string</td>
            <td><code>null</code></td>
            <td>No</td>
            <td>The filter used to exclude files within the specified source path.</td>
        </tr>
        <tr>
            <td>overwrite</td>
            <td>boolean</td>
            <td><code>false</code></td>
            <td>No</td>
            <td>A value indicating whether or not to overwrite existing files.</td>
        </tr>
        <tr>
            <td>flatten</td>
            <td>boolean</td>
            <td><code>false</code></td>
            <td>No</td>
            <td>A value indicating whether or not to flatten the included files when writing to the destination path.</td>
        </tr>
    </tbody>
    <tfooter>
        <tr>
            <td colspan="5">All arguments are prefixed by <code>copy_</code>.</td>
        </tr>
    </tfooter>
    </table>
</div>

## Global Arguments

The `copy` shade does not use any global arguments.

## Examples

### Copy All Files

{% highlight sh %}
copy copy_src_path='/examples' copy_dst_path='dist/examples'
{% endhighlight %}

### Copy with Filter

{% highlight sh %}
copy copy_src_path='/examples' copy_dst_path='dist/examples' copy_include='**/*.examples.cs'
{% endhighlight %}

### Copy, Flatten, and Overwrite

{% highlight sh %}
copy copy_src_path='/examples' copy_dst_path='dist/examples' copy_overwrite='${ true }' copy_flatten='${ true }'
{% endhighlight %}