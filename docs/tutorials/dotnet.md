# Build and publish a project with .NET CLI

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

Dotnet project specifications are found in the `Prepare`, `Compile`, `Test`, `Package`, and `Publish`, and `Document`
target tasks.



To publish a project with the .NET CLI, developers will need to have code under a ```/src``` folder and tests under a
```tests``` folder.

Tests will be run with XUnit. Condo will look for tests with a specified ```[Purpose]``` and ```PurposeType```.
Supported PurposeTypes include:

* Unit
* Integration
* EndToEnd
* Performance

For E2E tests, a ```CollectionDefinition``` with ```E2ECollection``` must be specified.



[get-started]: get-started.md
[lifecycle]: ../concepts/lifecycle.md
