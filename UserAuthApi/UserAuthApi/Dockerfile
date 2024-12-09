# Use the .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the .csproj file and restore any dependencies (via dotnet restore)
COPY UserAuthApi.csproj ./
RUN dotnet restore

# Copy the rest of the code
COPY . ./

# Build the application
RUN dotnet build UserAuthApi.csproj --configuration Release --output /app/build

# Publish the app to the /app/publish directory
RUN dotnet publish UserAuthApi.csproj --configuration Release --output /app/publish

# Use the ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Set the entry point to the app
ENTRYPOINT ["dotnet", "UserAuthApi.dll"]