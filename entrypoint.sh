#!/bin/bash

echo "updating database ..."
cd liquibase

if [ "${ASPNETCORE_ENVIRONMENT}" = "Staging" ]; then
    echo "staging environment"
    liquibase update --defaultsFile=sta.liquibase.properties
else
    echo "production environment"
    liquibase update --defaultsFile=sta.liquibase.properties --context-filter="prod"
fi

cd ..
echo "... finished!"

echo "starting webapi..."
dotnet /app/webapi.dll

