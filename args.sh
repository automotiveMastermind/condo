#!/bin/bash -x

function props()
{
    args=''

    until [ -z "$1" ]; do
         if [ ${1:0:3} = '-p:' ]; then
            prop=${1:3}
            key=${prop%%=*}
            value=${prop##*=}
        else
            args+="$1 "
        fi

        shift
    done

    echo $args
}

props $*
