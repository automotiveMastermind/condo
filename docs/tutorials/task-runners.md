# Build and publish a project with task runners

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

Task runner project specifications are found in the `Prepare` target task.

## Prepare
In order to run task runners, use npm scripts. Condo will not automatically call Grunt or Gulp, but developers can
create an npm script called ```condo``` that calls their grunt or gulp pipelines and condo will run it accordingly.

In order to run task runners, condo will look for a `gulpfile.*` and a `gruntfile.*`. It will then look for the
executable that you have specified.


[get-started]: get-started.md
[lifecycle]: ../concepts/lifecycle.md
