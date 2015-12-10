---
layout: docs
title: update-self
group: shades
---

Updates the build scripts located at the root of the project structure.

## Contents
* Will be replaced with the ToC, excluding the "Contents" header
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

This uses no arguments.

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
    </tbody>
    </table>
</div>

## Examples

Condo can update itself by adding the following call within any target:

{% highlight sh %}
    update-self
{% endhighlight %}

The default lifecycle includes a target aptly named `#update-self` that calls this shade.

## See Also

There are no related shades.