#! /bin/bash

# find the script path
root=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

# set the dnx directory
dnxpath=~/.dnx
dnvmpath=$dnxpath/dnvm
dnvmsh=$dnvmpath/dnvm.sh
dnvmuri="https://raw.githubusercontent.com/aspnet/Home/dev/dnvm.sh"

# determine if dnx path exists
if ! test -f "$dnvmpath"; then
    # make the dnx directory
    mkdir -p "$dnvmpath"
fi

# determine if dnvm is available
if ! type dnvm > /dev/null 2>&1; then
    # determine if dnvm.sh exists in the usual place
    if ! test -f "$dnvmsh"; then
        # attempt to download it from curl
        result=$(curl -L -D - "$dnvmuri" -o "$dnvmsh" -# | grep "^HTTP/1.1" | head -n 1 | sed "s/HTTP.1.1 \([0-9]*\).*/\1/")
       
        # source it if it was successfully retrieved
        [[ $result == "200" ]] && chmod ugo+x "$dnvmsh"
    fi

    # source dnvm directly
    source "$dnvmsh"

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
