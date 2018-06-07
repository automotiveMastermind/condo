# Enviornment Variables

## Building condo

Variable            | Type     | What does it do
--------------------|----------|----------------
SKIP_NPM            | `bool`     | Skips tests for npm install
SKIP_BOWER          | `bool`     | Skips tests for bower install
SKIP_POLYMER        | `bool`     | Skips tests for polymer
---

## Building docker containers

Variable            | Type       | What does it do
--------------------|------------|----------------
AllowPublish        | `bool`     | Will publish docker images to specified docker registry regardless of other variable states
DockerOrganization  | `string`   | the string that will be prefixed to your docker tags `$DockerOrganization/your-project-name`
---
