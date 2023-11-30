#!/bin/bash

current_dir=$(pwd)
subdirectory="db"

while IFS= read -r line; do
  eval "$line"
done < .env

cp git-hooks/pre-commit .git/hooks
chmod +x .git/hooks/pre-commit

if docker ps | grep -q "$DB_CONTAINER_NAME"; then
  echo "$DB_CONTAINER_NAME is already running ..."
  echo "Removing it ..."
  docker-compose down
  docker container rm "$DB_CONTAINER_NAME"
else
  echo "$DB_CONTAINER_NAME is not running."
fi

sudo rm -rf "db/data" # removes local changes and replaces with last commit (maybe create seperate file for after pulling and one for only start local environment)
sudo mkdir -p "db/data"

docker-compose up -d
