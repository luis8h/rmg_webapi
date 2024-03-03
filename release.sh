#!/bin/bash

# Check if the version argument is provided
if [ $# -eq 0 ]; then
    echo "Usage: $0 <version>"
    exit 1
fi

# Check if the current branch is main
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
if [ "$CURRENT_BRANCH" != "main" ]; then
    echo "Error: This script can only be run on the main branch."
    exit 1
fi

VERSION="v$1"
REGISTRY="192.168.188.179:5000"
IMAGE_NAME="luis8h/rmg_webapi"

git add .
git commit -m "release: ${VERSION}"
git push
git tag $VERSION

docker login "$REGISTRY"
docker buildx build -t "$REGISTRY/$IMAGE_NAME:$VERSION" .
docker push "$REGISTRY/$IMAGE_NAME:$VERSION"
