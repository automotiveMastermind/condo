# condo [![Travis Build Status](https://img.shields.io/travis/PulseBridge/Condo.svg?label=travis)](https://travis-ci.org/PulseBridge/Condo) [![AppVeyor Build Status](https://img.shields.io/appveyor/ci/dmccaffery/condo.svg)](https://ci.appveyor.com/project/dmccaffery/condo) [![NuGet Version](https://img.shields.io/nuget/v/PulseBridge.Condo.svg?label=version)](https://www.nuget.org/packages/PulseBridge.Condo/) [![NuGet Downloads](https://img.shields.io/nuget/dt/PulseBridge.Condo.svg?label=downloads)](https://www.nuget.org/packages/PulseBridge.Condo/)

> A build system for [DNX](http://docs.asp.net/en/latest/dnx/index.html) projects.

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

The easiest way to start using Condo is to use the [Yeoman Condo Generator](https://github.com/PulseBridge/Generator-Condo), which will configure a new "solution" structure for use with the Condo build system.

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

### Documentation

For more information, please refer to the [official documentation](http://open.pulsebridge.com/condo).