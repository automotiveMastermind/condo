#!/usr/bin/env bash

# find the working path
WORKING_PATH=$(pwd)

CLR_INFO='\033[1;33m'       # BRIGHT YELLOW
CLR_FAILURE='\033[1;31m'    # BRIGHT RED
CLR_SUCCESS="\033[1;32m"    # BRIGHT GREEN
CLR_CLEAR="\033[0m"         # DEFAULT COLOR

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

    [ -e "$MSBUILD_LOG" ] && rm -f $MSBUILD_LOG
    [ -e "$MSBUILD_RSP" ] && rm -f $MSBUILD_RSP
    [ -e "$CONDO_LOG" ] && rm -f $CONDO_LOG

    exit $EXIT_CODE
}

safe-exec() {
    local CMD=$1
    shift

    local CMD_NAME=$(basename $CMD)
    $CMD "$@" 2>&1 >> $CONDO_LOG

    local EXIT_CODE=$?

    if [ $EXIT_CODE -ne 0 ]; then
        echo -e "${CLR_FAILURE}$CMD_NAME failed with exit code: $EXIT_CODE. Check '$CONDO_LOG' for additional information...${CLR_CLEAR}" 1>&2
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

        export DOTNET_INSTALL_DIR=$DOTNET_PATH
        chmod +x $DOTNET_INSTALL

        safe-exec $DOTNET_INSTALL --channel $DOTNET_CHANNEL --version $DOTNET_VERSION
    fi

    export PATH="$DOTNET_INSTALL_DIR:$PATH"
    success "dotnet-cli was installed..."
}

# restore and publish msbuild
install_msbuild() {
    if [ ! -d "$MSBUILD_PATH" ]; then
        # create the msbuild path
        mkdir -p $MSBUILD_PUBLISH
        mkdir -p $CONDO_PUBLISH

        # get the current runtime
        RUNTIME=`dotnet --info | grep "RID" | awk '{ print $2 }'`

        # TRICKY, HACK, TODO: this is a hack to resolve the runtime for macos sierra, which is not yet officially
        # supported with native runtime libraries
        if [ "$RUNTIME" = "osx.10.12-x64" ]; then
            # set the runtime to el capitan
            RUNTIME="osx.10.11-x64"
        fi

        # get the msbuild project file and replace the RUNTIME marker with the current runtime, then emit to the
        cat "$CONDO_PATH/Scripts/msbuild.json" | sed "s/RUNTIME/$RUNTIME/g" > $MSBUILD_PROJ

        # copy the nuget config to the build root
        cp "$CONDO_PATH/Scripts/nuget.config" $BUILD_ROOT

        # restore msbuild
        info "msbuild: restoring msbuild packages..."
        safe-exec dotnet restore $MSBUILD_PATH --verbosity minimal
        success "msbuild: restore complete"

        # publish msbuild
        info "msbuild: publishing msbuild system..."
        safe-exec dotnet publish $MSBUILD_PATH --output $MSBUILD_PUBLISH --runtime $RUNTIME
        success "msbuild: publish complete"

        # restore condo
        info "condo: restoring condo packages..."
        safe-exec dotnet restore $CONDO_PATH --verbosity minimal
        success "condo: restore complete"

        # publish condo
        info "condo: publishing condo tasks..."
        safe-exec dotnet publish $CONDO_PATH --output $CONDO_PUBLISH --runtime $RUNTIME
        success "condo: publish complete"
    else
        info "condo was already built: use --reset to get the latest version."
    fi
}

# continue testing for arguments
while [[ $# > 0 ]]; do
    case $1 in
        --verbosity)
            CONDO_VERBOSITY=$2
            shift
            ;;
        --no-color)
            CLR_INFO=
            CLR_SUCCESS=
            CLR_FAILURE=
            CLR_CLEAR=
            MSBUILD_DISABLE_COLOR="DisableConsoleColor"
            ;;
        *)
            break
            ;;
    esac
    shift
done

ARTIFACTS_ROOT="$WORKING_PATH/artifacts"
BUILD_ROOT="$WORKING_PATH/.build"

MSBUILD_PATH="$BUILD_ROOT/msbuild-cli"
MSBUILD_PROJ="$MSBUILD_PATH/project.json"
MSBUILD_PUBLISH="$MSBUILD_PATH/bin/publish"
MSBUILD_LOG="$BUILD_ROOT/condo.msbuild.log"
MSBUILD_RSP="$BUILD_ROOT/condo.msbuild.rsp"

CONDO_PATH="$BUILD_ROOT/condo"
CONDO_PUBLISH=$MSBUILD_PUBLISH
CONDO_LOG="$BUILD_ROOT/condo.log"

# create the artifacts root path if it does not already exist
[ ! -d "$ARTIFACTS_ROOT" ] && mkdir -p $ARTIFACTS_ROOT

# delete the log paths if they exist
[ -e "$CONDO_LOG" ] && rm -f $CONDO_LOG
[ -e "$MSBUILD_LOG" ] && rm -f $MSBUILD_LOG
[ -e "$MSBUILD_RSP" ] && rm -f $MSBUILD_RSP

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

install_dotnet
install_msbuild

CONDO_TARGETS="$CONDO_PATH/Targets"
CONDO_PROJ="$WORKING_PATH/condo.build"

if [ ! -e "$CONDO_PROJ" ]; then
    CONDO_PROJ="$CONDO_TARGETS/condo.build"
fi

cat > $MSBUILD_RSP <<END_MSBUILD_RSP
-nologo
"$CONDO_PROJ"
-p:CondoTargetsPath="$CONDO_TARGETS/"
-p:CondoTasksPath="$CONDO_PUBLISH/"
-fl
-flp:LogFile="$MSBUILD_LOG";Encoding=UTF-8;Verbosity=$CONDO_VERBOSITY
-clp:$MSBUILD_DISABLE_COLOR;Verbosity=$CONDO_VERBOSITY
END_MSBUILD_RSP

# write out msbuild arguments to the rsp
safe-join $'\n' "$@" >> $MSBUILD_RSP

info "Starting build..."
info "msbuild '$CONDO_PROJ' $MSBUILD_ARGS"

"$MSBUILD_PUBLISH/corerun" "$MSBUILD_PUBLISH/MSBuild.exe" @"$MSBUILD_RSP"
safe-exit $?