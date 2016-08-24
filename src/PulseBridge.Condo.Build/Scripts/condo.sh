#!/usr/bin/env bash

# find the working path
WORKING_PATH=$(pwd)

CLR_INFO='\033[1;33m'       # BRIGHT YELLOW
CLR_FAILURE='\033[1;31m'    # BRIGHT RED
CLR_SUCCESS="\033[1;32m"    # BRIGHT GREEN
CLR_CLEAR="\033[0m"         # DEFAULT COLOR

ARTIFACTS_ROOT="$WORKING_PATH/artifacts"
BUILD_ROOT="$WORKING_PATH/.build"

CONDO_PATH="$BUILD_ROOT/condo"
CONDO_PUBLISH="$CONDO_PATH/bin/publish"

MSBUILD_PATH="$BUILD_ROOT/msbuild-cli"
MSBUILD_PROJ="$MSBUILD_PATH/project.json"
MSBUILD_PUBLISH="$MSBUILD_PATH/bin/publish"

if [ ! -d "$ARTIFACTS_ROOT" ]; then
    mkdir -p $ARTIFACTS_ROOT
fi

CONDO_LOG="$ARTIFACTS_ROOT/condo.log"

if [ -e "$CONDO_LOG" ]; then
    rm -f "$CONDO_LOG"
fi

success() {
    echo -e "${CLR_SUCCESS}$@${CLR_CLEAR}"
    echo "SUCCESS: $@" >> "$CONDO_LOG"
}

failure() {
    echo -e "${CLR_FAILURE}$@${CLR_CLEAR}"
    echo "FAILURE: $@" >> "$CONDO_LOG"
}

info() {
    echo -e "${CLR_INFO}$@${CLR_CLEAR}"
    echo "INFO   : $@" >> "$CONDO_LOG"
}

exec() {
    local cmd=$1
    shift

    local cmdname=$(basename $cmd)
    $cmd "$@" 2>&1 >> $CONDO_LOG

    local exitCode=$?
    if [ $exitCode -ne 0 ]; then
        failure "$cmdname $@ failed with exit code $exitCode"
        echo -e "${CLR_FAILURE}check '$CONDO_LOG' for additional information...${CLR_CLEAR}" 1>&2
        # __end $exitCode
    fi
}

# set the dotnet install path
DOTNET_PATH=~/.dotnet

# determine if the dotnet install url is not already set
if [ -z "$DOTNET_INSTALL_URL" ]; then
    # set the dotnet install url to the 1.0.0 release
    DOTNET_INSTALL_URL="https://github.com/dotnet/cli/raw/rel/1.0.0/scripts/obtain/dotnet-install.sh"
fi

# determine if the dotnet install channel is not already set
if [ -z "$DOTNET_CHANNEL" ]; then
    # set the dotnet channel
    DOTNET_CHANNEL="rel-1.0.0"
fi

# determine if the dotnet version is not already set
if [ -z "$DOTNET_VERSION" ]; then
    # set the dotnet version
    DOTNET_VERSION="1.0.0-preview2-003121"
fi

# download dotnet
install_dotnet() {
    if [ ! -z "$SKIP_DOTNET_INSTALL" ]; then
        info "Skipping installation of dotnet-cli by request (SKIP_DOTNET_INSTALL IS SET)..."
    else
        DOTNET_TEMP=$(mktemp -d -t dotnet)
        DOTNET_INSTALL="$DOTNET_TEMP/dotnet-install.sh"

        retries=5

        until (wget -O "$DOTNET_INSTALL" "$DOTNET_INSTALL_URL" 2>/dev/null || curl -o "$DOTNET_INSTALL" -L "$DOTNET_INSTALL_URL" 2>/dev/null); do
            failure "Unable to retrieve dotnet-install: '$DOTNET_INSTALL_URL'"

            if [ "$retries" -le 0 ]; then
                exit 1
            fi

            retries=$((retries - 1))
            failure "Retrying in 10 seconds... attempts left: $retries"
            sleep 10s
        done

        export DOTNET_INSTALL_DIR=$DOTNET_PATH
        chmod +x "$DOTNET_INSTALL"

        exec $DOTNET_INSTALL --channel $DOTNET_CHANNEL --version $DOTNET_VERSION
    fi

    export PATH="$DOTNET_INSTALL_DIR:$PATH"
    success "dotnet-cli was installed..."
}

# capture the msbuild arguments
MSBUILD_ARGS=("$@")

# get the msbuild log
MSBUILD_LOG="$ARTIFACTS_ROOT/condo.msbuild.log"

# determine if the log file already exists
if [ -e "$MSBUILD_LOG" ]; then
    # delete the old log
    rm -f "$MSBUILD_LOG"
fi

# restore and publish msbuild
install_msbuild() {
    if [ ! -d "$MSBUILD_PATH" ]; then
        # create the msbuild path
        mkdir -p "$MSBUILD_PUBLISH"
        mkdir -p "$CONDO_PUBLISH"

        # get the current runtime
        RUNTIME=`dotnet --info | grep "RID" | awk '{ print $2 }'`

        # get the msbuild project file and replace the RUNTIME marker with the current runtime, then emit to the
        # msbuild path... NOTE: this has been removed as not all runtimes are supported currently
        cat "$CONDO_PATH/scripts/msbuild.json" | sed "s/RUNTIME/$RUNTIME/g" > "$MSBUILD_PROJ"

        # restore msbuild
        info "msbuild: restoring msbuild packages..."
        exec dotnet restore "$MSBUILD_PATH" -v Minimal
        success "msbuild: restore complete"

        # publish msbuild
        info "msbuild: publishing msbuild system..."
        exec dotnet publish "$MSBUILD_PATH" -o "$MSBUILD_PUBLISH"
        success "msbuild: publish complete"

        # restore condo
        info "condo: restoring condo packages..."
        exec dotnet restore "$CONDO_PATH" -v Minimal
        success "condo: restore complete"

        # publish condo
        info "condo: publishing condo tasks..."
        exec dotnet publish "$CONDO_PATH" -o "$CONDO_PUBLISH"
        success "condo: publish complete"
    else
        info "condo was already built: use --update or --reset to get the latest version."
    fi
}

install_dotnet
install_msbuild