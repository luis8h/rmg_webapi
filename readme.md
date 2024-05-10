# Recipe Manager web API

## dependencies
[Mapster](https://github.com/MapsterMapper/Mapster) (installed with: dotnet add package Mapster)

#### packages:
- dotnet add package System.IdentityModel.Tokens.Jwt
- dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.2
- dotnet add package Dapper --version 2.1.35
- dotnet add package Dapper.FluentMap --version 2.0.0
- dotnet add package SixLabors.ImageSharp

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


## staing
-   `./stage.sh`
-   go to server and run `docker pull <image-name-with-repo-name-and-tag>`
-   restart docker containers


## production environment

### docker-compose

```
version: '3.8'

services:
  rmg_db_pro:
    image: postgres:latest
    restart: always
    container_name: rmg_db_pro
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD}
      POSTGRES_DB: ${DB_NAME}
    volumes:
      - /var/dumps
      - ./data:/var/lib/postgresql/data
    env_file:
      - .env
    networks:
      - rmg-network
    ports:
      - ${DB_PORT}:5432

  rmg_adminer_pro:
    image: adminer
    container_name: rmg_adminer_pro
    restart: always
    ports:
      - ${ADMINER_PORT}:8080
    env_file:
      - .env
    networks:
      - rmg-network

  rmg_webapi_pro:
    image: localhost:5000/luis8h/rmg_webapi:v0.0.0
    restart: always
    container_name: rmg_webapi_pro
    ports:
      - 5297:80
    networks:
      - rmg-network
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      DB_PASSWORD: ${DB_PASSWORD}

networks:
  rmg-network:
    driver: bridge
```
