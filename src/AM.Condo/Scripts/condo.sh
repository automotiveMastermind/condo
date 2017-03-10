#!/usr/bin/env bash

# find the working path
WORKING_PATH=$(pwd)

CLR_INFO='\033[1;33m'       # BRIGHT YELLOW
CLR_FAILURE='\033[1;31m'    # BRIGHT RED
CLR_SUCCESS="\033[1;32m"    # BRIGHT GREEN
CLR_CLEAR="\033[0m"         # DEFAULT COLOR

ARTIFACTS_ROOT="$WORKING_PATH/artifacts"
CONDO_ROOT="$HOME/.am/condo"
SRC_ROOT="$CONDO_ROOT/.src"
BUILD_ROOT="$CONDO_ROOT/.build"

MSBUILD_LOG="$BUILD_ROOT/condo.msbuild.log"
MSBUILD_RSP="$BUILD_ROOT/condo.msbuild.rsp"

CONDO_PATH="$SRC_ROOT/src/AM.Condo"
CONDO_PUBLISH="$CONDO_ROOT/cli"
CONDO_LOG="$BUILD_ROOT/condo.log"
CONDO_TARGETS="$CONDO_PATH/Targets"
CONDO_PROJ="$WORKING_PATH/condo.build"
CONDO_VERBOSITY="normal"

success() {
    echo -e "${CLR_SUCCESS}$@${CLR_CLEAR}"
    echo "log  : $@" >> $CONDO_LOG
}

failure() {
    echo -e "${CLR_FAILURE}$@${CLR_CLEAR}"
    echo "err  : $@" >> $CONDO_LOG
}

info() {
    if [[ "$CONDO_VERBOSITY" != "quiet" && "$CONDO_VERBOSITY" != "minimal" ]]; then
        echo -e "${CLR_INFO}$@${CLR_CLEAR}"
        echo "log  : $@" >> $CONDO_LOG
    fi
}

safe-exit() {
    local EXIT_CODE=$1

    if [ $EXIT_CODE -eq 0 ]; then
        success "exiting with exit code: $EXIT_CODE"
    else
        failure "exiting with exit code: $EXIT_CODE"
    fi

    cp $MSBUILD_LOG $CONDO_LOG $MSBUILD_RSP $ARTIFACTS_ROOT 1>/dev/null 2>&1

    [ -e "$BUILD_ROOT" ] && rm -rf $BUILD_ROOT

    exit $EXIT_CODE
}

safe-exec() {
    local CMD=$1
    shift

    local CMD_NAME=$(basename $CMD)
    echo "$CMD $@" >> $CONDO_LOG
    $CMD "$@" 2>&1 >> $CONDO_LOG

    local EXIT_CODE=$?

    if [ $EXIT_CODE -ne 0 ]; then
        echo -e "${CLR_FAILURE}$CMD_NAME failed with exit code: $EXIT_CODE. Check '$ARTIFACTS_ROOT/condo.log' for additional information...${CLR_CLEAR}" 1>&2
        safe-exit $EXIT_CODE
    fi
}

safe-join() {
     local IFS="$1"
     shift
     echo "$*"
}

# download dotnet
install_dotnet() {
    if [ ! -z "$SKIP_DOTNET_INSTALL" ]; then
        info "Skipping installation of dotnet-cli by request (SKIP_DOTNET_INSTALL is set)..."
    else
        DOTNET_TEMP=$(mktemp -d)
        DOTNET_INSTALL="$DOTNET_TEMP/dotnet-install.sh"

        retries=5

        until (wget -O $DOTNET_INSTALL $DOTNET_INSTALL_URL 2>/dev/null || curl -o $DOTNET_INSTALL -L $DOTNET_INSTALL_URL 2>/dev/null); do
            failure "Unable to retrieve dotnet-install: '$DOTNET_INSTALL_URL'"

            if [ "$retries" -le 0 ]; then
                safe-exit 1
            fi

            retries=$((retries - 1))
            failure "Retrying in 10 seconds... attempts left: $retries"
            sleep 10s
        done

        success "Retrieved dotnet-install script..."

        chmod +x $DOTNET_INSTALL
        safe-exec $DOTNET_INSTALL --channel $DOTNET_CHANNEL --version $DOTNET_VERSION
        safe-exec rm -rf $DOTNET_TEMP
    fi

    export PATH="$DOTNET_INSTALL_DIR:$PATH"
    success "dotnet-cli was installed..."
}

# restore and publish msbuild
install_condo() {
    if [ ! -d "$CONDO_PUBLISH" ]; then
        # make the publish directory
        mkdir -p $CONDO_PUBLISH

        # get the current runtime
        RUNTIME=`dotnet --info | grep "RID" | awk '{ print $2 }'`

        # restore condo
        info "condo: restoring condo packages..."
        safe-exec dotnet restore $SRC_ROOT --runtime $RUNTIME --verbosity minimal
        success "condo: restore complete"

        # publish condo
        info "condo: publishing condo tasks..."
        safe-exec dotnet publish $CONDO_PATH --runtime $RUNTIME --output $CONDO_PUBLISH --verbosity minimal /p:GenerateAssemblyInfo=false
        success "condo: publish complete"
    else
        info "condo was already built: use --reset to get the latest version."
    fi
}

# continue testing for arguments
while [[ $# > 0 ]]; do
    case $1 in
        -v|--verbosity)
            CONDO_VERBOSITY=$2
            shift
            ;;
        -nc|--no-color)
            CLR_INFO=
            CLR_SUCCESS=
            CLR_FAILURE=
            CLR_CLEAR=
            MSBUILD_DISABLE_COLOR="DisableConsoleColor"
            ;;
        --username)
            FEED_USERNAME=$2
            shift
            ;;
        --password)
            FEED_PASSWORD=$2
            shift
            ;;
        *)
            break
            ;;
    esac
    shift
done

# create the artifacts root path if it does not already exist
[ ! -d "$ARTIFACTS_ROOT" ] && mkdir -p $ARTIFACTS_ROOT

# delete the log paths if they exist
[ -e "$BUILD_ROOT" ] && rm -rf $BUILD_ROOT

# determine if the dotnet install url is not already set
if [ -z "$DOTNET_INSTALL_URL" ]; then
    # set the dotnet install url to the 1.0.1 release
    DOTNET_INSTALL_URL="https://github.com/dotnet/cli/raw/rel/1.0.1/scripts/obtain/dotnet-install.sh"
fi

# determine if the dotnet install channel is not already set
if [ -z "${DOTNET_CHANNEL:-}" ]; then
    # set the dotnet channel
    DOTNET_CHANNEL="rel-1.0.1"
fi

# determine if the dotnet version is not already set
if [ -z "$DOTNET_VERSION" ]; then
    # set the dotnet version
    DOTNET_VERSION="1.0.1"
fi

# set the dotnet install path
if [ -z "$DOTNET_INSTALL_DIR/sdk/$DOTNET_VERSION" ]; then
    export DOTNET_INSTALL_DIR=~/.dotnet
fi

[ ! -d "$BUILD_ROOT" ] && mkdir -p $BUILD_ROOT

install_dotnet
install_condo

if [ ! -e "$CONDO_PROJ" ]; then
    CONDO_PROJ="$CONDO_TARGETS/condo.build"
fi

cat > $MSBUILD_RSP <<END_MSBUILD_RSP
-nologo
"$CONDO_PROJ"
-p:CondoTargetsPath="$CONDO_TARGETS/"
-p:CondoTasksPath="$CONDO_PUBLISH/"
-p:PackageFeedUsername="$FEED_USERNAME"
-p:PackageFeedPassword="$FEED_PASSWORD"
-fl
-flp:LogFile="$MSBUILD_LOG";Encoding=UTF-8;Verbosity=$CONDO_VERBOSITY
-clp:$MSBUILD_DISABLE_COLOR;Verbosity=$CONDO_VERBOSITY
END_MSBUILD_RSP

# write out msbuild arguments to the rsp
safe-join $'\n' "$@" >> $MSBUILD_RSP

cat $MSBUILD_RSP >> $CONDO_LOG

info "Starting build..."
info "msbuild '$CONDO_PROJ' $MSBUILD_ARGS"

"dotnet" "msbuild" @"$MSBUILD_RSP"
safe-exit $?
