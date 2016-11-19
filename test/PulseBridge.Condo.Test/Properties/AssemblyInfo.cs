using Xunit;

// disable parallelism for now due to issues with max process on macos (in open source, this is a pain)
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]