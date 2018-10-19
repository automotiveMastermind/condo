#!/bin/bash

if type "docker" 1>/dev/null 2>&1; then
    service cron start 1>/dev/null 2>&1
    /root/.am/condo/docker-gc.sh 1>/dev/null 2>&1
fi

if [ "$1" = "condo" ]; then
    shift && dotnet /root/.am/condo/condo.dll "$@"
    exit $?
fi

if [ "$1" = "agent" ]; then
    shift && ./start.sh "$@"
    exit $?
fi

exec "$@"
exit $?
