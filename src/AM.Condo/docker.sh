#!/bin/bash
set -e

if [ "$1" = 'condo' ]; then
    shift && condo "$@"
    exit $?
fi

exec "$@"
