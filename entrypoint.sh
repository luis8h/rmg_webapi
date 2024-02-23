#!/bin/bash

echo "updating database ..."
cd liquibase
liquibase update --defaultsFile=sta.liquibase.properties
cd ..
echo "... finished!"

echo "starting webapi"
dotnet /app/webapi.dll

