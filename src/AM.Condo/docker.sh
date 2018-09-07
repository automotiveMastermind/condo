#!/bin/bash

set -e

if [ "$1" = 'condo' ]; then
    shift && dotnet ~/.am/condo/condo.dll "$@"
    exit $?
fi

exec "$@"
