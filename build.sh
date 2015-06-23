#!/bin/bash

# find the script path
root=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
build=$root/build

# set the dnx directory
dnxpath=~/.dnx

# make the dnx directory
mkdir -p "$dnxpath"

# determine if dnvm is installed
if [ ! type dnvm 1>&- 2>&- ]; then
    # copy the dnvm script to the dnxpath
    cp "$build/dnvm.sh" "$dnxpath"
    
    # source the dnvm script
    source "$build/dnvm.sh"
    
    # update the dnvm script
    dnvm update-self
fi

# set the URL to nuget
url=http://www.nuget.org/nuget.exe

# download nuget if it doesn't already exist
if [ ! -f "$dnxpath/nuget.exe" ]; then
    wget -O "$dnxpath/nuget.exe" "$url" 1>&- 2>&- || curl -o "$dnxpath/nuget.exe" --location "$url" 1>&- 2>&-
fi

# define the path for nuget
nuget=$root/.nuget/nuget.exe

# determine if the .nuget directory exists
if [ ! -f "$nuget" ]; then
    # make the nuget directory
    mkdir `dirname "$nuget"`

    # copy nuget.exe from the cache
    cp "$dnxpath/nuget.exe" "$nuget"
fi

# determine if dnx is available
if [ ! type dnx 1>&- 2>&- ]; then
    # upgrade dnx to latest
    dnvm upgrade
fi

# set sake and make file paths
sake=$root/packages/Sake/tools/Sake.exe
includes=$root/src/build/sake
make=make.shade

# determine if sake is installed
if [ ! -f "$sake" ]; then
    # install sake using nuget (so we have additional options not supported by DNU)
    dnx "$nuget" install Sake -pre -o packages -ExcludeVersion
fi

# execute the build with sake
dnx "$sake" -I "$includes" -f "$make" "$@"