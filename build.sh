#! /bin/bash

# find the script path
root=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

# change to the root path
cd $root

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
nugeturi=http://www.nuget.org/nuget.exe
appdata=~/.config
nugetpath=$appdata/NuGet
nugetcmd=$nugetpath/nuget.exe

# determine if the nuget directory exists
if ! test -f "$nugetpath"; then
    # make the nuget directory
    mkdir -p "$nugetpath"    
fi

# download nuget if it doesn't already exist
if ! test -f "$nugetcmd"; then
    wget -O "$nugetcmd" "$nugeturi" 1>&- 2>&- || curl -o "$nugetcmd" --location "$nugeturi" 1>&- 2>&-
fi

# define the path for nuget
nugetpath=$root/.nuget
nuget=$root/.nuget/nuget.exe

# determine if the .nuget directory exists
if ! test -f "$nugetpath"; then
    # make the nuget directory
    mkdir -p "$nugetpath"    
fi

# determine if the nuget exe exists
if ! test -f "$nuget"; then
    # copy nuget.exe from the cache
    cp "$nugetcmd" "$nuget"
fi

# upgrade dnx to latest
dnvm upgrade -r coreclr
dnvm use default

# set sake and make file paths
sources="https://www.myget.org/F/pulsebridge/api/v2;https://www.nuget.org/api/v2"
sake=$root/packages/Sake/tools/Sake.exe
includes=$root/src/build/sake
make=make.shade

# determine if sake is installed
if ! test -f "$sake"; then
    # install sake using nuget (so we have additional options not supported by DNU)
    mono "$nuget" install Sake -pre -o packages -ExcludeVersion
fi

# determine if the condo build includes exist
if ! test -f "$includes"; then
    mono "$nuget" install PulseBridge.Condo -pre -o packages -ExcludeVersion -Source $sources -NonInteractive
fi

# execute the build with sake
mono "$sake" -I "$includes" -f "$make" "$@"