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

# determine if mono is installed on the system
if ! type mono > /dev/null 2>&1; then
    # write a message to the user letting them know that mono should be installed
    echo
    echo 'Mono is currently a prerequisite in order to use Condo due to the dependency on Sake (https://github.com/sakeproject/sake), which is not supported within the Core CLR currently.'
    echo

    # get the name of the kernel
    uname=$(uname)

    # determine if the kernel is OS X
    if [[ $uname == "Darwin" ]]; then
        echo 'The recommended way to install Mono on OS X is to use Homebrew. Install Homebrew from: http://brew.sh then execute the following command to install mono.'
        echo
        echo 'brew install mono'
    else
        echo 'The recommended way to install Mono on Linux distributions is to follow the instructions available at: http://www.mono-project.com/download/'
    fi

    # exit with an error code
    exit 1
fi

# determine if dnx path exists
if ! test -d "$dnvmpath"; then
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
nugeturi=https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
appdata=~/.config
nugetpath=$appdata/NuGet
nugetcmd=$nugetpath/nuget.exe

# determine if the nuget directory exists
if ! test -d "$nugetpath"; then
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
if ! test -d "$nugetpath"; then
    # make the nuget directory
    mkdir -p "$nugetpath"
fi

# determine if the nuget exe exists
if ! test -f "$nuget"; then
    # copy nuget.exe from the cache
    cp "$nugetcmd" "$nuget"
fi

# determine if DNX exists on the path
if ! test -f "dnx"; then
  # upgrade to latest version of the core clr
  dnvm upgrade -r coreclr
fi

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
if ! test -d "$includes"; then
    #
    mono "$nuget" install PulseBridge.Condo -pre -o packages -ExcludeVersion -Source $sources -NonInteractive
fi

# execute the build with sake
mono "$sake" -I "$includes" -f "$make" "$@"