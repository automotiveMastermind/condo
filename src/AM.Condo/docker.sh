#!/bin/bash

set -e

if [ "$1" = 'condo' ]; then
    shift && dotnet /condo/condo.dll "$@"
    exit $?
fi

exec "$@"
