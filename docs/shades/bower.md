---
layout: shade
title: Bower
group: shades
---

Executes a bower package manager command

## Contents
* Will be replaced with the ToC, excluding the "Contents" header
{:toc}

### Supported Operating Systems

* Windows
* OS X
* Linux

### Arguments

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
            <td>bower_args</td>
            <td>string</td>
            <td><code>null</code></td>
            <td><strong>Yes</strong></td>
            <td>The arguments to pass to the bower command line tool.</td>
        </tr>
        <tr>
            <td>bower_options</td>
            <td>string</td>
            <td><code>${env:BOWER_OPTIONS}</code></td>
            <td>No</td>
            <td>Additional options to use when executing the bower command line tool</td>
        </tr>
        <tr>
            <td>bower_path</td>
            <td>string</td>
            <td><code>${global:working_path}</code></td>
            <td>No</td>
            <td>The base path in which to execute bower.</td>
        </tr>
        <tr>
            <td>bower_wait</td>
            <td>boolean</td>
            <td><code>true</code></td>
            <td>No</td>
            <td>A value indicating whether or not to wait for bower to exit before continuing.</td>
        </tr>
        <tr>
            <td>bower_quiet</td>
            <td>boolean</td>
            <td><code>${global:quiet}</code></td>
            <td>No</td>
            <td>A value indicating whether or not to suppress standard output when executing the bower command line tool.</td>
        </tr>
    </tbody>
    </table>
</div>

### Global Arguments

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

### Related

* one
* two
* three

### Examples

blah

#### Install all packages defined in bower.json
{% highlight sh %}
#install-bower-packages
    bower bower_args='install'
{% endhighlight %}

#### Install Bootstrap via Bower and update the bower.json file with the dependency
{% highlight sh %}
#example
    bower bower_args='install bootstrap' bower_options='--save-dev'
{% endhighlight %}

#### Install Bootstrap via Bower quietly and do not wait for exit
{% highlight sh %}
#example
    bower bower_args='install bootstrap' bower_quiet='${ true }' bower_wait='${ false }'
{% endhighlight %}