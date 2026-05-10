# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.slnx ./
COPY SedaWears.API/*.csproj SedaWears.API/
COPY SedaWears.Application/*.csproj SedaWears.Application/
COPY SedaWears.Domain/*.csproj SedaWears.Domain/
COPY SedaWears.Infrastructure/*.csproj SedaWears.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /src/SedaWears.API
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SedaWears.API.dll"]
