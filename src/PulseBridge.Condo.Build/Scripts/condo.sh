#!/usr/bin/env bash

# determine if the dotnet install url is not already set
if test -z "$DOTNET_INSTALL_URL"; then
    # set the dotnet install url to the 1.0.0 release
    DOTNET_INSTALL_URL="https://github.com/dotnet/cli/blob/rel/1.0.0/scripts/obtain/dotnet-install.sh"
fi

if test -z "$DOTNET_SDK_VERSION"; then
    DOTNET_SDK_VERSION="1.0.0-preview2-003121"
fi