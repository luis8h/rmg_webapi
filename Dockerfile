### webapi

# Use an official .NET runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

# Set the working directory in the container to /app
WORKDIR /app

# Expose ports 80 and 443 in the container
EXPOSE 80
EXPOSE 443

# Pull the .NET 7.0 SDK image from Microsofts container registry
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory in the container to /src
WORKDIR /src

# Copy the source code from the host machine to the container
COPY ["./webapi/webapi.csproj", "./"]
# Restore Nuget packages for the project
RUN dotnet restore "./webapi.csproj"
# Copy the rest of your application code to the container
COPY ./webapi ./

WORKDIR "/src/."
# Build the project in Release configuration and output the application to /app/build
RUN dotnet build "webapi.csproj" -c Release -o /app/build
# Name the current stage of the build as 'publish'
FROM build AS publish
# Publish the applicationin Release configuration to a folder named 'publish'
RUN dotnet publish "webapi.csproj" -c Release -o /app/publish
# NAme the current stage of the build as final
FROM base AS final
# Change working directory in the container to '/app'
WORKDIR /app
# Copy the application from the 'publish' stage to the current stage
COPY --from=publish /app/publish .



### liquibase
WORKDIR /

# Install Liquibase
RUN apt-get update && \
    apt-get install -y default-jre && \
    apt-get clean

RUN apt install gpg wget -y
RUN wget -O- https://repo.liquibase.com/liquibase.asc | gpg --dearmor > liquibase-keyring.gpg
RUN cat liquibase-keyring.gpg | tee /usr/share/keyrings/liquibase-keyring.gpg > /dev/null
RUN echo 'deb [arch=amd64 signed-by=/usr/share/keyrings/liquibase-keyring.gpg] https://repo.liquibase.com stable main' | tee /etc/apt/sources.list.d/liquibase.list

RUN apt-get update
RUN apt-get install liquibase

# Copy Liquibase files to the container
COPY ./db/ /liquibase
COPY ./entrypoint.sh entrypoint.sh



### entrypoint

# Set the command to run Liquibase update before starting the API
# WORKDIR /app
# ENTRYPOINT [ "sh", "-c", "liquibase update --changeLogFile=../liquibase/db.changelog-root.xml --defaultsFile=../liquibase/sta.liquibase.properties && dotnet webapi.dll" ]

# ENTRYPOINT [ "sh", "-c", "dotnet webapi.dll" ]
ENTRYPOINT ["/entrypoint.sh"]
