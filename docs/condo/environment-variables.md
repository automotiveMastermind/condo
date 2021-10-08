# Environment Variables

## Condo

Variable            | Type     | What does it do
--------------------|----------|----------------
SKIP_NPM            | `bool`   | Skips all npm build steps
SKIP_BOWER          | `bool`   | Skips all bower build steps
SKIP_DOTNET         | `bool`   | Skips all dotnet build steps
SKIP_GO             | `bool`   | Skips all go build steps
---

## Dotnet

Variable            | Type     | What does it do
--------------------|----------|----------------
SKIP_DOTNET_RESTORE | `bool`   | Skips restoring packages
SKIP_DOTNET_BUILD   | `bool`   | Skips building
SKIP_DOTNET_TEST    | `bool`   | Skips testing
SKIP_DOTNET_PUBLISH | `bool`   | Skips publishing project
SKIP_DOTNET_PUSH    | `bool`   | Skips pushing nuget packages
---

## Go

Variable            | Type     | What does it do
--------------------|----------|----------------
SKIP_GO_INSTALL     | `bool`   | Skips restoring packages
SKIP_GO_BUILD       | `bool`   | Skips building
SKIP_GO_TEST        | `bool`   | Skips testing
SKIP_GO_PUBLISH     | `bool`   | Skips publishing project
---

## Building docker containers

Variable            | Type       | What does it do
--------------------|------------|----------------
AllowPublish        | `bool`     | Will publish docker images and Nuget packages to specified docker registry regardless of other variable states
DockerOrganization  | `string`   | the string that will be prefixed to your docker tags `$DockerOrganization/your-project-name`
---
