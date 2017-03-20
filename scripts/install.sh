#!/usr/bin/env bash

CONDO_ROOT="$HOME/.am/condo"
CONDO_LOG="$CONDO_ROOT/condo.log"
CONDO_VERBOSITY='normal'

CONDO_CLR_INFO='\033[1;33m'       # BRIGHT YELLOW
CONDO_CLR_FAILURE='\033[1;31m'    # BRIGHT RED
CONDO_CLR_SUCCESS="\033[1;32m"    # BRIGHT GREEN
CONDO_CLR_CLEAR="\033[0m"         # DEFAULT COLOR

__condo-success() {
    echo -e "${CONDO_CLR_SUCCESS}$@${CONDO_CLR_CLEAR}"
    echo "log  : $@" >> $CONDO_LOG
}

__condo-failure() {
    echo -e "${CONDO_CLR_FAILURE}$@${CONDO_CLR_CLEAR}"
    echo "err  : $@" >> $CONDO_LOG
}

__condo-info() {
    if [[ "$CONDO_VERBOSITY" != 'quiet' && "$CONDO_VERBOSITY" != 'minimal' ]]; then
        echo -e "${CONDO_CLR_INFO}$@${CONDO_CLR_CLEAR}"
        echo "log : $@" >> $CONDO_LOG
    fi
}

__condo-install-help() {
    echo 'Installation for Condo Build System'
    echo
    echo 'Usage:'
    echo '  ./condo.sh install [arguments] [common-options]'
    echo
    echo 'Common options:'
    echo '  -h|-?|--help        print this help information'
    echo '  -l|--log            location of the installation log'
    echo '                        DEFAULT: $CONDO_INSTALL_DIR/condo.log'
    echo
    echo 'Arguments:'
    echo '  -nc|--no-color      do not emit color to output, which is useful for capture'
    echo '  -id|--install-dir   location in which to install condo'
    echo '                        DEFAULT: $HOME/.am/condo'
    echo '  -r|--reset          reinstall condo'
    echo '  -u|--update         update to the latest version of condo'
    echo '                        NOTE: this argument is effected by the branch argument; the version will be determined by the latest commit available on the specified branch.'
    echo '  -b|--branch         install condo from the specified branch'
    echo '                        DEFAULT: master'
    echo '  -s|--source         install condo from the specified source path (local)'
    echo '  --uri               install condo from the specified URI'
    echo
    echo 'EXAMPLE:'
    echo '  ./condo.sh install --branch feature/cli --no-color --install-dir $HOME/.condo --log $HOME/condo.log'
    echo '    - installs condo from the `feature/cli` branch'
    echo '    - no color will be emitted to the console (either STDOUT or STDERR)'
    echo '    - condo will be installed to `$HOME/.condo`'
    echo '    - the installation log will be saved to $HOME/condo-install.log'
}

__condo-help() {
    echo 'Condo Build System'
    echo
    echo 'Usage:'
    echo '  ./condo.sh [host-options] [command] [arguments] [common-options]'
    echo
    echo 'Host options:'
    echo '  --version           display version number'
    echo '  --info              display info about the host and condo build system'
    echo
    echo 'Arguments:'
    echo '  [host-options]      options passed to the host (dotnet)'
    echo '  [command]           the command to execute'
    echo '  [arguments]         options passed to the `install` command'
    echo '  [common-options]    options common to all commands'
    echo
    echo 'Common options:'
    echo '  -h|-?|--help        print this help information'
    echo '  -l|--log            location of the installation log'
    echo '                        DEFAULT: $CONDO_INSTALL_DIR/condo.log'
    echo '  -nc|--no-color      do not emit color to output, which is useful for capture'
    echo
    echo 'Commands:'
    echo '  install             installs condo on the local system'
    echo '  update              updates condo to the latest version'
    echo '  init                initializes condo in the current directory'
    echo '  build               uses condo to execute the build target (Build)'
    echo '  test                uses condo to execute the test target (Test)'
    echo '  publish             uses condo to execute the publish target (Publish)'
    echo '  ci                  uses condo to execute the continuous integration target (CI)'
    echo '  clean               uses condo to execute the clean target'
    echo
    echo 'Advanced commands:'
    echo '  nuget               uses condo to manipulate nuget feeds and credentials'
    echo '  conventions         uses condo to create new conventions'
    echo '  config              edit the condo configuration'
}

__condo-install() {
    # get the current path
    local CURRENT_PATH="$pwd"

    # find the script path
    local ROOT_PATH=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

    # setup well-known condo paths
    local CLI_ROOT="$CONDO_ROOT/cli"
    local SRC_ROOT="$CONDO_ROOT/src"
    local CONDO_SHELL="$SRC_ROOT/src/AM.Condo/Scripts/condo.sh"

    # change to the root path
    pushd $ROOT_PATH 1>/dev/null

    # write a newline for separation
    echo

    # test for help command
    case $1 in
        -h|-\?|--help)
            __condo-help
            exit 0
            ;;
    esac

    # continue testing for arguments
    while [[ $# > 0 ]]; do
        case $1 in
            -h|-\?|--help)
                __condo-help
                exit 0
                ;;
            -r|--reset)
                local CONDO_RESET=1
                ;;
            -l|--local)
                local CONDO_LOCAL=1
                ;;
            -u|--update)
                local CONDO_UPDATE=1
                ;;
            --uri)
                local CONDO_URI=$2
                shift
                ;;
            -b|--branch)
                local CONDO_BRANCH=$2
                shift
                ;;
            -s|--source)
                local CONDO_SOURCE=$2
                shift
                ;;
            -id|--install-dir)
                CONDO_ROOT="$1"

                if [ -z "${CONDO_LOG_SET:-}" ]; then
                    CONDO_LOG="$CONDO_ROOT/condo-install.log"
                fi

                shift
                ;;
            -l|--log)
                CONDO_LOG="$1"
                local CONDO_LOG_SET=1
                shift
                ;;
            -nc|--no-color)
                local CONDO_CLR_INFO=
                local CONDO_CLR_FAILURE=
                local CONDO_CLR_CLEAR=
                break
                ;;
        esac
        shift
    done

    if [[ -d "$CONDO_ROOT" && "$CONDO_RESET" = "1" ]]; then
        __condo-info 'Resetting condo build system...'
        rm -rf "$CONDO_ROOT"
    fi

    if [ -d "$CLI_ROOT" ]; then
        echo 'Condo was already installed. Use `--reset` to force remove.'
        return 0
    fi

    if [ -z "${DOTNET_INSTALL_DIR:-}" ]; then
        export DOTNET_INSTALL_DIR=~/.dotnet
    fi

    if [ -z "${CONDO_BRANCH:-}" ]; then
        CONDO_BRANCH='master'
    fi

    if [ -z "${CONDO_URI:-}" ]; then
        CONDO_URI="https://github.com/automotivemastermind/condo/tarball/$CONDO_BRANCH"
    fi

    if [ "$CONDO_LOCAL" = "1" ]; then
        CONDO_SOURCE="$ROOT_PATH"
    fi

    if [ ! -d "$SRC_ROOT" ]; then
        __condo-info "Creating path for condo at $CONDO_ROOT..."
        mkdir -p $SRC_ROOT

        if [ ! -z $CONDO_SOURCE ]; then
            __condo-info "Using condo build system from $CONDO_SOURCE..."
            cp -r $CONDO_SOURCE/* $SRC_ROOT/
            cp -r $CONDO_SOURCE/template $CONDO_ROOT
        else
            local CURL_OPT='-s'
            if [ ! -z "${GH_TOKEN:-}" ]; then
                CURL_OPT='$CURL_OPT -H "Authorization: token $GH_TOKEN"'
            fi

            __condo-info "Using condo build system from $CONDO_URI..."

            CONDO_TEMP=$(mktemp -d)
            CONDO_TAR="$CONDO_TEMP/condo.tar.gz"

            retries=5

            until (wget -O $CONDO_TAR $CONDO_URI 2>/dev/null || curl -o $CONDO_TAR --location $CONDO_URI 2>/dev/null); do
                __condo-failure "Unable to retrieve condo: '$CONDO_URI'"

                if [ "$retries" -le 0 ]; then
                    exit 1
                fi

                retries=$((retries - 1))
                __condo-failure "Retrying in 10 seconds... attempts left: $retries"
                sleep 10s
            done

            CONDO_EXTRACT="$CONDO_TEMP/extract"
            CONDO_SOURCE="$CONDO_EXTRACT"

            mkdir -p $CONDO_EXTRACT
            tar xf $CONDO_TAR --strip-components 1 --directory $CONDO_EXTRACT
            cp -r $CONDO_SOURCE/* $SRC_ROOT/
            cp -r $CONDO_SOURCE/template $CONDO_ROOT
            rm -Rf $CONDO_TEMP
        fi
    fi

    # ensure that the condo shell is executable
    chmod a+x $CONDO_SHELL

    # write a newline for separation
    echo

    # change to the original path
    popd
}

__condo-install "$@"

# capture the current exit code
EXIT_CODE=$?

# exit
exit $EXIT_CODE
