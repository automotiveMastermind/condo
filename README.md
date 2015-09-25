# condo

> A build system for [DNX][dnx-url] projects.

## Vitals

Info          | Badges
--------------|--------------
Version       | [![Version][release-v-image]][release-url] [![NuGet Version][nuget-v-image]][nuget-url]
License       | [![License][license-image]][license]
Downloads     | [![NuGet Downloads][nuget-d-image]][nuget-url]
Build Status  | [![Travis Build Status][travis-image][travis-url] [![AppVeyor Build Status][appveyor-image]][appveyor-url]

## Getting Started

### What is Condo?

Condo is a cross-platform command line interface (CLI) build system for projects using the .NET Core Framework. It is capable of automatically detecting and executing all of the steps
necessary to make a DNX project function correctly, including, but not limited to:

* Automatic semantic versioning
* Restoring package manager dependencies (NuGet, NPM, Bower)
* Executing default task runner commands (Grunt, Gulp)
* Compiling projects and test projects
* Executing unit tests
* Packing NuGet packages
* Pushing (Publishing) NuGet packages

These are just some of the most-used features of the build system.

### Using Condo

The easiest way to start using Condo is to use the [Yeoman Condo Generator][yo-url], which will configure a new "solution" structure for use with the Condo build system.

1. Make sure that you have Yeoman installed:

	```bash
	npm install -g yo
	```

2. Install the Condo generator:

	```bash
	npm install -g generator-condo
	```

3. Initiate the generator:

	```bash
	yo condo
	```

4. Run the build:

	OS X / Linux:

	```bash
	./build.sh
	```

	Windows (CLI):

	```cmd
	build
	```

	Windows (PoSH):
	```posh
	./build.ps1
	```

## Documentation

For more information, please refer to the [official documentation][docs-url].

## Copright and License

&copy;. PulseBridge, Inc. and contributors. Distributed under the APACHE 2.0 license. See [LICENSE][] and [CREDITS][] for details.

[license-image]: //img.shields.io/badge/license-APACHE%202.0-blue.svg
[license]: LICENSE
[credits]: CREDITS

[release-url]: //github.com/pulsebridge/condo/releases/latest
[release-v-image]: //img.shields.io/github/release/pulsebridge/condo.svg?style=flat-square

[travis-url]: //travis-ci.org/pulsebridge/condo
[travis-image]: //img.shields.io/travis/pulsebridge/condo.svg?label=travis

[appveyor-url]: //ci.appveyor.com/project/dmccaffery/condo
[appveyor-image]: //img.shields.io/appveyor/ci/dmccaffery/condo.svg?label=appveyor

[nuget-url]: //www.nuget.org/packages/pulsebridge.condo
[nuget-v-image]: //img.shields.io/nuget/v/pulsebridge.condo.svg?label=version
[nuget-d-image]: //img.shields.io/nuget/dt/pulsebridge.condo.svg?label=downloads

[yo-url]: //www.npmjs.com/package/generator-condo

[dnx-url]: http://docs.asp.net/en/latest/dnx/index.html
[docs-url]: http://open.pulsebridge.com/condo