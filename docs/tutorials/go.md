# Build and publish a project with Go

First refer to docs for [getting started with condo][get-started].

This tutorial will cover the default implementation of condo (default implementation lifecycle docs found
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

Go project specifications are found in the `Prepare`, `Compile`, `Test`, `Publish` target tasks.

## Prepare

In the `Prepare` target task, condo looks for a modules file with the extension `.mod`. This will signify that this project is a golang project. If your golang project does not use modules then add an empty file named `go.condo` to the root of your repository.

Condo will then find the go cli executable and finally install any missing dependencies.

Condo will execute:

```bash
go install
```

prepare target environmental variables:

| variable            | description              |
|---------------------|--------------------------|
| SKIP_GO             | If set to `true` condo will skip the execution of all go targets |
| SKIP_GO_INSTALL     | If set to `true` then `go install` will be skipped |
| GO_INSTALL_OPTIONS  | Set additional install options. Will be executed as `go install $GO_INSTALL_OPTIONS` |

## Compile

In the `Compile` target task, condo will build the project so that it can be executed on the local machine.
> This target will only execute when the `CI` flag is set

condo will execute:

```bash
    go build -o "artifacts/build/"
```

compile target environmental variables:

| variable            | description              |
|---------------------|--------------------------|
| SKIP_GO_BUILD       | If set `true` then build will be skipped |
| GO_BUILD_OPTIONS    | Set additional build options. `go build $GO_BUILD_OPTIONS` |

## Test

In the `Test` target task, condo will execute tests if tests were found in your go project.

condo will execute:

```bash
go test
```

test target environmental variables:

| variable            | description              |
|---------------------|--------------------------|
| SKIP_GO_TEST        | If set `true` tests will be skipped |
| GO_TEST_OPTIONS     | Set additional test options. Will be executed as `go test $GO_TEST_OPTIONS` |

## Publish

In the `Publish` target task, condo will build the project. Condo will use `GO_BUILD_TARGETS` variable for selecting the OS and architecture the build will produce. The variable should be set `os/arch` and be semi-colon delimited.

For example if you wanted to build for OSX amd64 and Windows Arm you would set the variable as `darwin/amd64;windows/arm`. Condo will loop over each `os/arch` and build them.

>Condo will output the `os/arch` combinations into the `artifacts/publish/os/arch` folder.

condo will execute (pseudo code):

```bash
foreach GO_BUILD_TARGETS:
    env GOOS=OS GOARCH=ARCH go build -o "artifacts/publish/OS/ARCH"
```

publish target environmental variables:

| variable            | description              |
|---------------------|--------------------------|
| SKIP_GO_BUILD       | If set `true` then publish will be skipped |
| GO_BUILD_TARGETS    | Set the target `os/arch` to build for. Delimite with `;` for multiple
| GO_BUILD_OPTIONS    | Set additional build options. `go build $GO_BUILD_OPTIONS` |

[get-started]: get-started.md
[lifecycle]: ../concepts/lifecycle.md
