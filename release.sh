#!/bin/bash

# Check if the version argument is provided
if [ $# -eq 0 ]; then
    echo "Usage: $0 <version>"
    exit 1
fi

VERSION="v$1"
REGISTRY="192.168.188.179:5000"
IMAGE_NAME="luis8h/rmg_webapi"

docker login "$REGISTRY"
docker buildx build -t "$REGISTRY/$IMAGE_NAME:$VERSION" ./webapi
docker push "$REGISTRY/$IMAGE_NAME:$VERSION"
