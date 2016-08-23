#!/usr/bin/env bash

# find the root path
ROOT_PATH=$(pwd)

echo $ROOT_PATH

# set the dotnet install path
DOTNET_PATH="~/.dotnet"

# determine if the dotnet install url is not already set
if [ -z "$DOTNET_INSTALL_URL" ]; then
    # set the dotnet install url to the 1.0.0 release
    DOTNET_INSTALL_URL="https://github.com/dotnet/cli/blob/rel/1.0.0/scripts/obtain/dotnet-install.sh"
fi

# determine if the dotnet install channel was already set
if [ -z "$DOTNET_CHANNEL" ]; then
    # set the dotnet channel
    DOTNET_CHANNEL="rel-1.0.0"
fi

# determine if the dotnet version is set
if [ -z "$DOTNET_VERSION" ]; then
    # set the dotnet version
    DOTNET_VERSION="1.0.0-preview2-003121"
fi

# capture the msbuild arguments
MSBUILD_ARGS=("$@")

# download dotnet
install_dotnet() {
     echo yes
}

# restore and publish msbuild
install_msbuild() {
    echo yes
}


echo "yay"