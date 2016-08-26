#!/usr/bin/env bash

# get the current path
CURRENT_PATH=$(pwd)

# find the script path
ROOT_PATH=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

# change to the root path
cd $ROOT_PATH

# determine if condo.sh already exists
if [ -f condo-local.sh ]; then
    # delete it
    rm -f condo-local.sh
fi

# copy the template to the local path
cp template/condo.sh condo-local.sh

# run condo using local build
CONDO_SHELL="$ROOT_PATH/condo-local.sh"
$CONDO_SHELL --reset --local $@

# remove the local condo file
rm -f condo-local.sh

# change back to the current directory
cd $CURRENT_PATH