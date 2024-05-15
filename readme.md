# Recipe Manager web API

This is a .NET Core webapi connected to a postgresql database which is managed
with liqubase.
Note that the commands below might only work on my local setup and are for me
to remember the commands.
The project is still in development and by far not finished.

## dependencies
#### packages:
- ```dotnet add package System.IdentityModel.Tokens.Jwt```
- ```dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.2```
- ```dotnet add package Dapper --version 2.1.35```
- ```dotnet add package Dapper.FluentMap --version 2.0.0```
- ```dotnet add package SixLabors.ImageSharp```
- ```dotnet add package Mapster```

## db
### liquibase
#### commands
-   rollback to a db version: ```liquibase rollback --tag='v0.0.1' --labelFilter='!tagset:v0.0.1'```
    (the reason for the labelfilter is that otherwise the specified changeset of the tag would also get reverted)

## start developing on new machine
-   `git clone https://github.com/luis8h/rmg_webapi.git`
-   `cd rmg_webapi`
-   `git fetch`
-   `git switch <feature_branch_name>` (checkout to remote branch)
-   `cp template.env .env` (and type in db password in .evn file)

## staging
-   `./stage.sh`
-   go to server and run `docker pull <image-name-with-repo-name-and-tag>`
-   restart docker containers

## create new release
-   ```./release.sh <new-version>``` eg. ```./release.sh 0.0.6```
-   go to server and run `docker pull <image-name-with-repo-name-and-tag>`
-   restart docker containers
