#!/bin/bash
docker run -it -v $(pwd):/src --workdir /src condo bash -c ./condo.sh
