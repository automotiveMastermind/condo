# Build and publish a project with bower

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

Bower project specifications are found in the `Prepare` target task.

## Prepare
In the `Prepare` target, condo will look for a `bower.json` file and consequent executables.

Condo will then prepare and execute `bower install` with any options you might have specified.

[get-started]: get-started.md
[lifecycle]: ../concepts/lifecycle.md
