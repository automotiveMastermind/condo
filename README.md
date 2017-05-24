# condo

> A build system for \<any\> project.

## Vitals

Info          | Badges
--------------|--------------
Version       | [![Version][release-v-image]][release-url] [![NuGet Version][nuget-v-image]][nuget-url]
License       | [![License][license-image]][license]
Build Status  | [![Travis Build Status][travis-image]][travis-url] [![AppVeyor Build Status][appveyor-image]][appveyor-url]
Chat          | [![Join Chat][gitter-image]][gitter-url]
Issues        | [![Issues][issues-image]][issues-url]

## Getting Started

### What is Condo?

Condo is a cross-platform command line interface (CLI) build system for projects using NodeJS, CoreCLR, .NET Framework, or... well, anything.
It is capable of automatically detecting and executing all of the steps necessary to make <any> project function correctly, including, but not limited to:

* Automatic semantic versioning
* Restoring package manager dependencies (NuGet, NPM, Bower)
* Executing default task runner commands (Grunt, Gulp)
* Compiling projects and test projects (package.json and msbuild)
* Executing unit tests (xunit, mocha, jasmine, karma, protractor)
* Packing NuGet packages
* Pushing (Publishing) NuGet packages

These are just some of the most-used features of the build system.

### Using Condo
We are currently developing `condo-cli` to make bootstrapping your projects to use condo a snap.

But its not ready yet!..
So lets do it the old fashion way:

1. Get the nessesary files:

    Copy the four files in the `templates` folder and add them to the root folder of your project.
    ```
    condo.build
    condo.cmd
    condo.ps1
    condo.sh
    ```

2. Edit the `condo.build` config file:

    Configure some stuff, only if ya want

3. Run the build:

	OS X / Linux:

	```bash
	./condo.sh
	```

	Windows (CLI):

	```cmd
	condo
	```

	Windows (PoSH):
	```posh
	./condo.ps1
	```

### Helpful hints

If you are using any protected nuget feeds then run the following command to add your credentials

    condo -SecureFeed

You can also get the latest and greatest version of condo by running this command

    condo -Reset

## Documentation

For more information, please refer to the [official documentation][docs-url].

## Copright and License

&copy; automotiveMastermind and contributors. Distributed under the MIT license. See [LICENSE][] and [CREDITS][] for details.

[license-image]: https://img.shields.io/badge/license-MIT-blue.svg
[license]: LICENSE
[credits]: CREDITS

[release-url]: //github.com/automotivemastermind/condo/releases/latest
[release-v-image]:https://img.shields.io/github/release/automotivemastermind/condo.svg?style=flat-square&label=github

[travis-url]: //travis-ci.org/automotivemastermind/condo
[travis-image]: https://img.shields.io/travis/automotivemastermind/condo.svg?label=travis

[appveyor-url]: //ci.appveyor.com/project/automotivemastermind/condo
[appveyor-image]: https://img.shields.io/appveyor/ci/automotivemastermind/condo.svg?label=appveyor

[nuget-url]: //www.nuget.org/packages/automotivemastermind.condo
[nuget-v-image]: https://img.shields.io/nuget/v/automotivemastermind.condo.svg?label=nuget
[nuget-d-image]: https://img.shields.io/nuget/dt/automotivemastermind.condo.svg?label=nuget

[yo-url]: //www.npmjs.com/package/generator-condo

[docs-url]: //automotivemastermind.github.io/condo

[gitter-url]: //gitter.im/automotivemastermind/condo
[gitter-image]:https://img.shields.io/badge/⊪%20gitter-join%20chat%20→-1dce73.svg

[issues-url]: //waffle.io/automotivemastermind/condo
[issues-image]: https://badge.waffle.io/automotivemastermind/condo.svg?label=backlog&title=Backlog
