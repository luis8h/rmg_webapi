#!/bin/bash

sql_file="setup-db.sql" # Replace with the name of your SQL file

sudo rm -rf db/setup-script
sudo mkdir -p db/setup-script
sudo touch "db/setup-script/$sql_file"

docker-compose up -d

if [ -f .env ]; then
    while IFS='=' read -r key value; do
        export "$key"="$value"
    done < .env
else
  echo ".env file not found!"
  exit 1
fi

retry() {
  docker exec "$DB_CONTAINER_NAME" psql -U postgres -c "SELECT 1" > /dev/null 2>&1
  return $?
}

echo "Waiting for PostgreSQL to be ready..."
until retry; do
  echo "PostgreSQL is not ready yet..."
  sleep 6
done

echo "PostgreSQL is ready!"

docker exec "$DB_CONTAINER_NAME" pg_dump -U postgres -d "$DB_NAME" -f "/var/dumps/$sql_file"
sudo docker cp "$DB_CONTAINER_NAME:/var/dumps/$sql_file" "db/setup-script/$sql_file"
