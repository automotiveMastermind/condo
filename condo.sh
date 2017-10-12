#!/usr/bin/env bash
export CONDO_BUILD_PATH=$(pwd)

#pretty colors
CLR_INFO='\033[1;33m'       # BRIGHT YELLOW
CLR_FAILURE='\033[1;31m'    # BRIGHT RED
CLR_CLEAR="\033[0m"         # DEFAULT COLOR

#Known paths
CONDO_ROOT="$HOME/.am/condo/unix"
CONDO_BRANCH="develop"
CONDO_URI="https://raw.githubusercontent.com/automotiveMastermind/condo"
CONDO_DOCKER_URI="$CONDO_URI/$CONDO_BRANCH/docker-compose-unix.yml"
CONDO_RUN_URI="$CONDO_URI/$CONDO_BRANCH/run/condo.sh"

function failure() {
    echo -e "${CLR_FAILURE}$@${CLR_CLEAR}"
}

function info() {
        echo -e "${CLR_INFO}$@${CLR_CLEAR}"
}

# cleanup code even if script fails
function finish() {
    rm condo-local.sh
}

# get file from remote repo if does not already exist locally. files are cached via latest git SHA
function getFile() {

    local SHA_URI="https://api.github.com/repos/automotivemastermind/condo/commits/$CONDO_BRANCH"
    local CONDO_SHA=$(curl -s $SHA_URI | grep sha | head -n 1 | sed 's#.*\:.*"\(.*\).*",#\1#')
    local SHA_PATH=$CONDO_ROOT/$CONDO_SHA

    if [[ -f $SHA_PATH && ! -f $CONDO_ROOT/$2 && ! "$CONDO_RESET" = "1" ]]; then
        info "condo: latest version already installed: $CONDO_SHA"
        return 0
    fi

    touch $SHA_PATH

    retries=3
    until (curl -o $CONDO_ROOT/$2 --create-dirs $1 2>/dev/null || wget -O $CONDO_ROOT/$2 $1 2>/dev/null); do
        failure "Unable to retrieve condo: '$1'"

        if [ "$retries" -le 0 ]; then
            return 1
        fi

        retries=$((retries - 1))
        failure "Retrying in 5 seconds... attempts left: $retries"
        sleep 5s
    done
    info "latest version installed $CONDO_SHA"
}

CONDO_OPTS=()

# continue testing for arguments
while [[ $# > 0 ]]; do
    case $1 in
        -r|--reset)
            CONDO_RESET=1
            info "condo: reset flag detected will pull latest files"
            ;;
        -l|--local)
            CONDO_LOCAL=1
            info "condo: local flag detected, lets build condo with condo"
            ;;
        --)
            CONDO_OPTS+=("$1")
            shift
            break
            ;;
        *)
            CONDO_OPTS+=("$1")
            ;;
    esac
    shift
done

# quote msbuild params since docker-compose run removes the slash
CONDO_OPTS+=("${@@Q}")

### setup condo project
if [ "$CONDO_LOCAL" = "1" ]; then
    # copy local condo.sh to local build root
    cp $CONDO_BUILD_PATH/run/condo.sh $CONDO_BUILD_PATH/condo-local.sh
    # cleanup file on exit
    trap finish EXIT
    # remove all files in condo root
    rm $CONDO_ROOT/*
    #copy local compose file to condo root
    cp $CONDO_BUILD_PATH/docker-compose-unix.yml $CONDO_ROOT/docker-compose.yml
else
    # check if condo is cached or get it from repo
    getFile $CONDO_RUN_URI condo.sh
    # copy cached condo into project as condo-local.sh
    cp $CONDO_ROOT/condo.sh $CONDO_BUILD_PATH/condo-local.sh
    # cleanup file on exit
    trap finish EXIT
    # check if condo is cached or get it from repo
    getFile $CONDO_DOCKER_URI docker-compose.yml
fi

# Does thou have the docker?
docker --version | grep "Docker version" > /dev/null
if [ $? -eq 0 ]; then
    info "condo: running docker"
    # run condo in docker container
    docker-compose -f $CONDO_ROOT/docker-compose.yml run condo "${CONDO_OPTS[@]}"
else
    info "condo: docker is not installed falling back to local build"
    # run condo
    ./condo-local.sh "${CONDO_OPTS[@]}"
fi
