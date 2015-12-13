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

The `update-self` shade does not accept any arguments.

## Global Arguments

The following global arguments are used by `update-self`:

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

### Update Self

{% highlight sh %}
    update-self
{% endhighlight %}

The default lifecycle includes a target aptly named `update-self` that calls this shade. This target can be called by executing condo as follows:

#### OS X, Linux
{% highlight sh %}
./condo.sh update-self
{% endhighlight %}

#### Windows
{% highlight cmd %}
condo.cmd update-self
{% endhighlight %}