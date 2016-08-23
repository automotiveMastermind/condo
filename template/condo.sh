#!/usr/bin/env bash

# get the current path
CURRENT_PATH=$(pwd)

# find the script path
ROOT_PATH=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

# change to the root path
cd $ROOT_PATH

# write a newline for separation
echo

# function used to print help information for condo
condo_help() {
    echo "condo-help"
}

# continue testing for arguments
while [[ $# > 0 ]]; do
    case $1 in
        -h|-\?|--help)
            condo_help
            exit 0
            ;;
        -r|--condo-reset)
            CONDO_RESET=1
            ;;
        -l|--condo-local)
            CONDO_LOCAL=1
            ;;
        -u|--condo-uri)
            CONDO_URI=$2
            shift
            ;;
        -b|--condo-branch)
            CONDO_BRANCH=$2
            shift
            ;;
        -p|--condo-path)
            CONDO_SOURCE=$2
            shift
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

CONDO_ROOT="$ROOT_PATH/.condo"
CONDO_SHELL="$CONDO_ROOT/src/scripts/condo.sh"

if [ -z "$CONDO_BRANCH" ]; then
    CONDO_BRANCH="develop"
fi

if [ -z "$CONDO_URI" ]; then
    CONDO_URI="https://github.com/pulsebridge/condo/tarball/$CONDO_BRANCH"
fi

if [[ -d $CONDO_ROOT && "$CONDO_RESET" = "1" ]]; then
    echo -e "Resetting condo build system..."
    rm -Rf "$CONDO_ROOT"
fi

if [ "$CONDO_LOCAL" = "1" ]; then
    echo -e "Using local condo build system..."
    CONDO_ROOT="$ROOT_PATH/.."
    CONDO_SHELL="$CONDO_ROOT/src/PulseBridge.Condo.Build/scripts/condo.sh"
fi

if [ ! -d "$CONDO_ROOT" ]; then
    echo -e "Creating path for condo at $CONDO_ROOT..."
    mkdir -p "$CONDO_ROOT"

    if ! [ -z $CONDO_SOURCE ]; then
        echo -e "Using condo build system from $CONDO_SOURCE..."
        cp -R "$CONDO_SOURCE" "$CONDO_ROOT"
    else
        echo -e "Using condo build system from $CONDO_URI..."

        CONDO_TEMP=$(mktemp -d)
        CONDO_TAR="$CONDO_TEMP/condo.tar.gz"

        retries=5

        until (wget -O "$CONDO_TAR" "$CONDO_URI" 2>/dev/null || curl -o "$CONDO_TAR" --location "$CONDO_URI" 2>/dev/null); do
            echo -e "Unable to retrieve condo: '$CONDO_TAR'"

            if [ "$retries" -le 0 ]; then
                exit 1
            fi

            retries=$((retries - 1))
            echo "Retrying in 10 seconds... attempts left: $retries"
            sleep 10s
        done

        mkdir "$CONDO_ROOT"
        tar xf "$CONDO_TAR" --strip-components 1 --directory "$CONDO_ROOT"
        rm -Rf "$CONDO_TEMP"
    fi
fi

# ensure that the condo shell is executable
chmod a+x "$CONDO_SHELL"

# execute condo shell with the arguments
"$CONDO_SHELL" "$@"

# write a newline for separation
echo

# change to the original path
cd $CURRENT_PATH