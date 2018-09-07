#!/bin/bash

set -e

dotnet ~/.am/condo/condo.dll "$@"
exit $?
