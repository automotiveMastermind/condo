# Build and publish a project with NPM

First refer to docs for [getting started with condo][get-started].

Once you have the four necessary files (`condo.build`, `condo.cmd`, `condo.ps1`, `condo.sh`) you are ready to set up
your project to utilize condo.

This tuturial will cover the default implementation of condo (default implementation lifecycle docs found
[here][lifecycle]).

For reference, condo's default lifecycle is as follows:

1. Clean
2. Initialize
3. Version
4. Prepare
5. Compile
6. Test
7. Package
8. Build

Node project specifications are found in the `Prepare`, `Compile`, and `Test` target tasks.

## Prepare
In the `Prepare` target task, condo looks for a `package.json`. Developers can create an npm script called `condo`
that calls their grunt/gulp/build pipelines, or specify a `build` script. Condo will use `condo` or `build` and also
look for a `test` script.

## Compile
In the `Compile` target task,

## Test
In the `Test` target task,


[get-started]: get-started.md
[lifecycle]: ../concepts/lifecycle.md
