#!/bin/bash

set -e

dotnet /condo/condo.dll "$@"
exit $?
