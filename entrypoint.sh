#!/bin/bash

echo "updating database ..."
cd liquibase

if [ "${ASPNETCORE_ENVIRONMENT}" = "Staging" ]; then
    echo "staging environment"
    # liquibase update --defaultsFile=sta.liquibase.properties
    liquibase update \
        --url="jdbc:postgresql://rmg_db_sta:5432/rmg_db" \
        --username="postgres" \
        --password="${DB_PASSWORD}" \
        --defaultsFile=sta.liquibase.properties
else
    echo "production environment"
    liquibase update \
        --url="jdbc:postgresql://rmg_db_pro:5432/rmg_db" \
        --username="postgres" \
        --password="${DB_PASSWORD}" \
        --defaultsFile=sta.liquibase.properties \
        --context-filter="prod"
fi

cd ..
echo "... finished!"

echo "starting webapi..."
cd app
dotnet webapi.dll

