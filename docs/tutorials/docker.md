# Build and publish a docker container

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

Docker project specifications are found in the `Prepare`, `Compile`, and `Publish` target tasks.


## Prepare
In the `Prepare` target, condo will look for a few files and respective executables at the root of your repository or
in a `Docker` folder:
1. `dockerfile`
2. `dockerfile.debug`
3. `docker-compose`

Condo will apply your specifications and options to prepare:

```bash
docker build
```

```bash
docker compose
```

```bash
docker push
```


## Compile
In the `Compile` target, condo will take your specifications and execute:

```bash
docker build --label %(DockerMetadata.Label) --tag %(DockerMetadata.VersionLabel) -f %(DockerMetadata.Identity) %(DockerMetadata.ProjectDir)
```

```bash
docker tag %(DockerTags.VersionLabel) %(DockerTags.Identity)
```

## Publish
In the `Publish` target, condo will take your specifications and execute:

```bash
docker tag %(DockerRegistryTags.VersionLabel) $(DockerOrganization)%(DockerRegistryTags.Identity)
```

```bash
docker push $(DockerOrganization)%(DockerRegistryTags.Identity)
```


[get-started]: get-started.md
[lifecycle]: ../concepts/lifecycle.md
