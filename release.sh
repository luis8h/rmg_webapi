#!/bin/bash

# Check if the version argument is provided
if [ $# -eq 0 ]; then
    echo "Usage: $0 <version>"
    exit 1
fi

VERSION="v$1"
REGISTRY="192.168.188.179:5000"
IMAGE_NAME="luis8h/rmg_webapi"

# Check if the current branch is main
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
if [ "$CURRENT_BRANCH" != "main" ]; then
    echo "Error: This script can only be run on the main branch."
    exit 1
fi

# Check if any existing tag matches the version
if git tag --list | grep -q "$VERSION"; then
    echo "Error: A tag with a version-like name already exists. Please ensure the version is unique."
    exit 1
fi

# Docker login
docker login "$REGISTRY"
if [ $? -ne 0 ]; then
    echo "Error: Docker login failed."
    exit 1
fi

# Build and push Docker image
docker buildx build -t "$REGISTRY/$IMAGE_NAME:$VERSION" .
if [ $? -ne 0 ]; then
    echo "Error: Docker build failed."
    exit 1
fi

docker push "$REGISTRY/$IMAGE_NAME:$VERSION"
if [ $? -ne 0 ]; then
    echo "Error: Docker push failed."
    exit 1
fi

# Git operations
git add .
git commit -m "release: ${VERSION}"
git tag $VERSION
git push origin main

