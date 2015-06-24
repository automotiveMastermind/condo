#!/bin/bash

# find the script path
root=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
build=$root/src/build

# set the dnx directory
dnxpath=~/.dnx
dnvmpath=$dnxpath/dnvm

# determine if dnx path exists
if ! test -f "$dnvmpath"; then
    # make the dnx directory
    mkdir -p "$dnvmpath"
fi

confirm() {
    while true; do

        if [ "${2:-}" = "Y" ]; then
            prompt="[y]/n"
            default=Y
        elif [ "${2:-}" = "N" ]; then
            prompt="y/[n]"
            default=N
        else
            prompt="y/n"
            default=
        fi

        read -p "$1 [$prompt] " REPLY </dev/tty

        if [ -z "$REPLY" ]; then
            REPLY=$default
        fi

        case "$REPLY" in
            Y*|y*) return 0 ;;
            N*|n*) return 1 ;;
        esac
    done
}

# determine if dnvm is available
if ! type dnvm > /dev/null 2>&1; then

    # determine if dnvm.sh exists in the usual place
    if ! test -f "$dnvmpath/dnvm.sh"; then
        # copy the dnvm script to the dnxpath
        cp "$build/dnvm.sh" "$dnvmpath"

        # ask to source the script from the bash profile
        if confirm "Would you like to source dnvm from your profile?" Y; then
            # determine if the bash profile does not exist
            if ! test -f "~/.bash_profile"; then
                # create the bash profile file
                touch ~/.bash_profile
            fi

            # emmit source dnvm in bash profile
            echo ". $dnvmpath/dnvm.sh" >> ~/.bash_profile
        fi
    fi

    # source dnvm directly
    source "$dnvmpath/dnvm.sh"

    # update the dnvm script
    dnvm update-self
fi

# set the URL to nuget
url=http://www.nuget.org/nuget.exe

# download nuget if it doesn't already exist
if ! test -f "$dnxpath/nuget.exe"; then
    wget -O "$dnxpath/nuget.exe" "$url" 1>&- 2>&- || curl -o "$dnxpath/nuget.exe" --location "$url" 1>&- 2>&-
fi

# define the path for nuget
nuget=$root/.nuget/nuget.exe

# determine if the .nuget directory exists
if ! test -f "$nuget"; then
    # make the nuget directory
    mkdir `dirname "$nuget"`

    # copy nuget.exe from the cache
    cp "$dnxpath/nuget.exe" "$nuget"
fi

# determine if dnx is available
if ! type dnx > /dev/null 2>&1; then
    # upgrade dnx to latest
    dnvm upgrade
fi

# set sake and make file paths
sake=$root/packages/Sake/tools/Sake.exe
includes=$root/src/build/sake
make=make.shade

# determine if sake is installed
if ! test -f "$sake"; then
    # install sake using nuget (so we have additional options not supported by DNU)
    dnx "$nuget" install Sake -pre -o packages -ExcludeVersion
fi

# execute the build with sake
dnx "$sake" -I "$includes" -f "$make" "$@"