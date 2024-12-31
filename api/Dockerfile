FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

EXPOSE 80

# Copy solution and project files first
COPY *.sln .
COPY api/*.csproj api/
COPY library/*.csproj library/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY api/ api/
COPY library/ library/

# Build and publish
WORKDIR /src/api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "api.dll"]