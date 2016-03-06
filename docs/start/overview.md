---
layout: docs
title: Overview
group: start
redirect_from: "/start/"
---

Condo is a cross-platform continuous integration system for building, testing, publishing, and deploying *any* project. It is
built from the ground up to quickly enable continous integration in just about any environment.

## Contents

* Will be replaced with the ToC, excluding the "Contents" header
{:toc}

## Why Condo

Modern applications have a myriad of technologies at their disposal. The landscape of the open-source community has led to a world
where we all have a Swiss army knife of options when it comes to picking the right set of tools for the job. Having so many choices is
mostly a good thing, but it comes at the cost of complexity. As an example, these docs are written, built, and deployed using the
following:

* Bootstrap v4.0.0-alpha, coming from bower
* FontAwesome, also coming from bower
* Jekyll and plugins, coming from Ruby Gems
* Lunr, a client-side search engine, coming from npm
* Sass transpiler from npm
* TypeScript transpiler from npm
* TypeScript definitions coming from tsd
* ... etc.

Of course, one could simply copy the css, scripts, and other dependencies from the distribution folders, or by downloading the code from each
website hosting the tools we want, but that creates a very static situation. Various package managers exist for a reason, and we want to be
able to take advantage of them.

So, now we have this complicated pipeline of tools coming from an increasingly vast array of sources; how do we tie it together? We
use a task runner, of course. We chose gulp for its simplicity... yet another dependency. We even wrote our gulp file using TypeScript
-- **why** -- because *reasons*!

Crap. Now I need to build it on a CI server -- does it have Ruby? What about NodeJS? or Bower? or... or... *gasp*. This is where Condo
comes into play. Condo will automatically find things to do during the build lifecycle, and decides when it makes sense to do them. If
it discovers a `gulpfile.babel.js` file in the `src/myproject.ng/app/somewhere_over_the_rainbow` folder, for example, it can reason about
the following:

* The gulpfile is named with `babel`, so we need to make sure that the babel transpiler for EcmaScript 6 is present.
* Babel is a dependency that is deployed using npm -- does the system have NodeJS present?
    - No -- download a copy of it and install it *locally* so we can continue.
* Install the babel dependencies
* Ask gulp if it has a default task
* Execute the default task if it has one

While many build systems are capable of solving a lot of the same problems, they usually do so in different ways. If you want to build
an app on both Windows and Linux, for instance, one would have to write duplicate scripts that targets each environment. These
scripts are combersome and complicated to write, and usually need to be updated as new dependencies come into play.

## How Condo Works

The build system is implemented using two components that come together to offer a generic out-of-the-box experience that should
prove capable for most projects. the first of these components is the lifecycle. The second component, called goals, implements that lifecycle.
The actions performed by the goals are called shades. The entry point of the build system, called `condo.shade` will define which lifecycle, goals,
and actions to perform.

### Lifecycle

The [default lifecycle]({{site.baseurl}}/start/default-lifecycle) defines a workflow through which a build will progress. Each step in the workflow is called
a `target`. A target can depend on another target, which creates a dependency. Each target will only be executed once, regardless of how many other
targets may depend on it. This ensures that targets are executed in the appropriate order without requiring much effort from the user. Simply define
what targets must be executed before the current target can be executed successfully, and the build system will take care of the rest. While the
default lifecycle does not include any implementation, it does come with a predefined pipeline, complete with recommended dependencies, that should
make it more than adequate for even the most complicated projects.

### Goals

The [default goals]({{site.baseurl}}/start/default-goals) acts as a best-effort implementation of the default lifecycle. These goals should enable the
vast majority of applications to be built, tested, and published with little to no configuration effort. Once Condo has been included at the root
of a project or solution, these goals will automatically detect what to do by scanning the project structure for key files, such as `project.json`,
`package.json`, `bower.json`, `tsd.json`, `gulpfile.*`, `grunt.*`, `Gemfile`, etc.

Simply create projects underneath this root path as you normally would, and the build system will *usually* just do the right thing. To make the
build system work best, we have defined a set of simple [best practices](/start/best-practices) to follow.

### Shades

The foundational actions that can be performed at any stage of the build lifecycle are called shades. The [shades]({{site.baseurl}}/shades)
made available for use by Condo are [thoroughly documented]({{site.baseurl}}/shades).

### Entry Point

The entry point of the build system is defined by a specialized file called `condo.shade` that must exist at the root of the project where
the `condo.sh` and `condo.cmd` files live. An example `condo.shade` file is included below:

{% highlight md %}
var product             = 'MyProduct'
var version             = '1.0.0'
var company             = 'MyCompany'
var copyright           = '©. MyCompany. All rights reserved.'
var license             = 'MIT'
var licenseUri          = 'https://opensource.org/licenses/MIT'

var src_path            = 'src'
var test_path           = 'test'

use-condo-lifecycle
use-condo-goals
{% endhighlight %}

This file should prove to be self explanatory, but we'll go through them one line at a time:

Variable      | Description
--------------|------------
product       | The name of the product, which may incorporate one or more projects.
version       | The version of the product currently being developed. SemVer is automated by the default goals as [defined here]({{site.baseurl}}/semantic-versioning).
company       | The name of the company or organization that owns the product.
copyright     | The copyright notice for the product.
license       | The SPDX license ID of the associated license, or a proprietary name for the license.
licenseUri    | A URL where the full license text for the product can be read.
src_path      | The path, relative to the directory where the `condo.shade` file exists, in which source code is maintained.
test_path     | The path, relative to the directory where the `condo.shade` file exists, in which test code is maintained.

The next two lines import the [default lifecycle]({{site.baseurl}}/start/lifecycle) and [default goals]({{site.baseurl}}/start/goals).