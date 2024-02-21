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
COPY ["webapi.csproj", "./"]
# Restore Nuget packages for the project
RUN dotnet restore "./webapi.csproj"
# Copy the rest of your application code to the container
COPY . .
WORKDIR "/src/."
# Build the project in Release configuration and output the application to /app/build
RUN dotnet build "webapi.csproj" -c Release -o /app/build
# Name the current stage of the build as 'publish'
FROM build AS publish
# Publish the applicationin Release configuration to a folder named 'publish'
RUN dotnet publish "webapi.csproj" -c Release -o /app/publish



### liquibase



### final setup

# NAme the current stage of the build as final
FROM base AS final
# Change working directory in the container to '/app'
WORKDIR /app
# Copy the application from the 'publish' stage to the current stage
COPY --from=publish /app/publish .
# Set the command that will be run when Docker container is started
ENTRYPOINT ["dotnet", "webapi.dll"]
