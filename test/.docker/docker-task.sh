#!/bin/bash
set -e

if [ "$1" = 'condo' ]; then
    exec /condo/condo.sh "$@"
fi

exec "$@"
