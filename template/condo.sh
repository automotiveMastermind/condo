#!/usr/bin/env bash

CLR_INFO='\033[1;33m'       # BRIGHT YELLOW
CLR_FAILURE='\033[1;31m'    # BRIGHT RED
CLR_SUCCESS="\033[1;32m"    # BRIGHT GREEN
CLR_CLEAR="\033[0m"         # DEFAULT COLOR

# get the current path
CURRENT_PATH=$(pwd)

# find the script path
ROOT_PATH=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

# change to the root path
cd $ROOT_PATH

# write a newline for separation
echo

success() {
    echo -e "${CLR_SUCCESS}$@${CLR_CLEAR}"
}

failure() {
    echo -e "${CLR_FAILURE}$@${CLR_CLEAR}"
}

info() {
    if [[ "$CONDO_VERBOSITY" != "quiet" && "$CONDO_VERBOSITY" != "minimal" ]]; then
        echo -e "${CLR_INFO}$@${CLR_CLEAR}"
    fi
}

# function used to print help information for condo
condo_help() {
    echo -e "condo-help"
}

# continue testing for arguments
while [[ $# > 0 ]]; do
    case $1 in
        -h|-\?|--help)
            condo_help
            exit 0
            ;;
        --reset|--update)
            CONDO_RESET=1
            ;;
        --local)
            CONDO_LOCAL=1
            ;;
        --uri)
            CONDO_URI=$2
            shift
            ;;
        --branch)
            CONDO_BRANCH=$2
            shift
            ;;
        --path)
            CONDO_SOURCE=$2
            shift
            ;;
        --verbosity)
            CONDO_VERBOSITY=$2
            break
            ;;
        --no-color)
            CLR_INFO=
            CLR_SUCCESS=
            CLR_FAILURE=
            CLR_CLEAR=
            break
            ;;
        --)
            shift
            break
            ;;
        *)
            break
            ;;
    esac
    shift
done

BUILD_ROOT="$ROOT_PATH/.build"
CONDO_ROOT="$BUILD_ROOT/condo"
CONDO_SHELL="$CONDO_ROOT/Scripts/condo.sh"

if [ -z "$CONDO_BRANCH" ]; then
    CONDO_BRANCH="develop"
fi

if [ -z "$CONDO_URI" ]; then
    CONDO_URI="https://github.com/pulsebridge/condo/tarball/$CONDO_BRANCH"
fi

if [[ -d "$BUILD_ROOT" && "$CONDO_RESET" = "1" ]]; then
    info "Resetting condo build system..."
    rm -rf "$BUILD_ROOT"
fi

if [ "$CONDO_LOCAL" = "1" ]; then
    CONDO_SOURCE="$ROOT_PATH/src/PulseBridge.Condo.Build"
fi

if [ ! -d "$CONDO_ROOT" ]; then
    info "Creating path for condo at $CONDO_ROOT..."
    mkdir -p $CONDO_ROOT

    if [ ! -z $CONDO_SOURCE ]; then
        info "Using condo build system from $CONDO_SOURCE..."
        cp -r $CONDO_SOURCE/* $CONDO_ROOT/
    else
        info "Using condo build system from $CONDO_URI..."

        CONDO_TEMP=$(mktemp -d)
        CONDO_TAR="$CONDO_TEMP/condo.tar.gz"

        retries=5

        until (wget -O $CONDO_TAR $CONDO_URI 2>/dev/null || curl -o $CONDO_TAR --location $CONDO_URI 2>/dev/null); do
            failure "Unable to retrieve condo: '$CONDO_TAR'"

            if [ "$retries" -le 0 ]; then
                exit 1
            fi

            retries=$((retries - 1))
            failure "Retrying in 10 seconds... attempts left: $retries"
            sleep 10s
        done

        tar xf $CONDO_TAR --strip-components 1 --directory $CONDO_ROOT
        rm -Rf $CONDO_TEMP
    fi
fi

# ensure that the condo shell is executable
chmod a+x $CONDO_SHELL

# execute condo shell with the arguments
$CONDO_SHELL $@

# capture the current exit code
EXIT_CODE=$?

# write a newline for separation
echo

# change to the original path
cd $CURRENT_PATH

# exit
exit $EXIT_CODE