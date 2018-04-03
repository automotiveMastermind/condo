# Build and publish a project with Polymer

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

Polymer project specifications are found in the `Prepare`, `Compile`, and `Test` target tasks.

## Prepare
In the `Prepare` target task, condo will look for a `polymer.json` file and the specified executables. It will then
prepare:

```bash
polymer install
```

```bash
polymer lint
```

```bash
polymer build
```

```bash
polymer test
```

## Compile
In the `Compile` target task, condo will execute:
```bash
polymer lint
```

```bash
polymer build
```


## Test
In the `Test` target task, condo will execute

```bash
polymer test
```


[get-started]: get-started.md
[lifecycle]: ../concepts/lifecycle.md
