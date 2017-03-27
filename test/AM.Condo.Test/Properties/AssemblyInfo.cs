// <copyright file="AssemblyInfo.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

using Xunit;

// disable parallelism for now due to issues with max process on macos (in open source, this is a pain)
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
