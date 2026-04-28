FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["SedaWears.API/SedaWears.API.csproj", "SedaWears.API/"]
COPY ["SedaWears.Application/SedaWears.Application.csproj", "SedaWears.Application/"]
COPY ["SedaWears.Domain/SedaWears.Domain.csproj", "SedaWears.Domain/"]
COPY ["SedaWears.Infrastructure/SedaWears.Infrastructure.csproj", "SedaWears.Infrastructure/"]
RUN dotnet restore "SedaWears.API/SedaWears.API.csproj"

COPY . .
RUN dotnet publish "SedaWears.API/SedaWears.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080

ENTRYPOINT ["dotnet", "SedaWears.API.dll"]
