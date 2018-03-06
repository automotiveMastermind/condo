# Publish a project with .NET CLI

To publish a project with the .NET CLI, developers will need to have code under a ```/rc``` folder and tests under a ```tests``` folder.

Tests will be run with XUnit. Condo will look for tests with a specified ```[Purpose]``` and ```PurposeType```. Supported PurposeTypes include:

* Unit
* EndToEnd

For E2E tests, a ```CollectionDefinition``` with ```E2ECollection``` must be specified.
