# Recipe Manager web API

## dependencies
-   [Mapster](https://github.com/MapsterMapper/Mapster) (installed with: dotnet add package Mapster)

## db
### liquibase
#### commands
-   rollback to a db version: ```liquibase rollback --tag='v0.0.1' --labelFilter='!v0.0.1'```
    (the reason for the labelfilter is that otherwise the specified tag would also get reverted)


## production environment

### docker-compose

```
version: '3.8'

services:
  rmg_db_pro:
    image: postgres:latest
    restart: always
    container_name: rmg_db
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
    container_name: rmg_adminer
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
    container_name: rmg_webapi
    ports:
      - 5297:80
    networks:
      - rmg-network
    environment:
      - ASPNETCORE_ENVIRONMENT=Production

networks:
  rmg-network:
    driver: bridge
```
