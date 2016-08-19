#!/usr/bin/env bash

# get the current path
CURRENT_PATH=$(pwd)

# find the script path
ROOT_PATH=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

# change to the root path
cd $ROOT_PATH

# run condo using local build
./template/condo.sh --condo-local