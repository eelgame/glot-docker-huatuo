#!/bin/bash
cd "$(dirname "$0")"

docker-compose run --rm -v $(pwd)/glot-www/glot-www:/build www-build /build/build.sh
docker-compose build
docker-compose up