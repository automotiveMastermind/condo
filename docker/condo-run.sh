#!/bin/bash
docker run -it -v $(pwd):/src --workdir /src condo-unix bash -c ./condo.sh
