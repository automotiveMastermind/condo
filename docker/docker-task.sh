#!/bin/bash
set -e

if [ "$1" = 'condo' ]; then
    shift
    exec /condo/condo.sh "$@"
fi

exec "$@"
