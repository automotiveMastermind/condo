#!/bin/bash
CLR_INFO='\033[1;33m'       # BRIGHT YELLOW
CLR_FAILURE='\033[1;31m'    # BRIGHT RED

#Known paths
CONDO_ROOT="$HOME/.am/condo/unix"
CONDO_URI="https://raw.githubusercontent.com/automotiveMastermind/condo"
CONDO_DOCKER_URI="$CONDO_URI/feature/docker-files-for-days/docker/unix/docker-compose.yml"
CONDO_LEGACY_URI="$CONDO_URI/feature/docker-files-for-days/template/condo.sh"

function failure() {
    echo -e "${CLR_FAILURE}$@${CLR_CLEAR}"
}

function info() {
        echo -e "${CLR_INFO}$@${CLR_CLEAR}"
}

function getFile {

    local SHA_URI="https://api.github.com/repos/automotivemastermind/condo/commits/develop"
    local CONDO_SHA=$(curl -s $SHA_URI | grep sha | head -n 1 | sed 's#.*\:.*"\(.*\).*",#\1#')
    local SHA_PATH=$CONDO_ROOT/$CONDO_SHA

    if [ -f $SHA_PATH ]; then
        info "condo: latest version already installed: $CONDO_SHA"
        return 0
    fi

    info $CONDO_SHA > $SHA_PATH

    retries=3
    until (wget -O $CONDO_ROOT $1 2>/dev/null || curl -o $CONDO_ROOT --location $CONDO_URI 2>/dev/null); do
        failure "Unable to retrieve condo: '$CONDO_URI'"

        if [ "$retries" -le 0 ]; then
            return 1
        fi

        retries=$((retries - 1))
        failure "Retrying in 5 seconds... attempts left: $retries"
        sleep 5s
    done
}


docker --version | grep "Docker version" > /dev/null
if [ $? -eq 0 ]; then
    info "Checking for latest compose..."
    getFile $CONDO_DOCKER_URI
    docker-compose -f /$CONDO_ROOT/docker-compose.yml run condo -- $@
else
    info "Docker is not installed falling back to legacy build"
    #pull condo script
    getFile CONDO_LEGACY_URI
    /$CONDO_ROOT/condo.sh -- $@
fi
