# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY src/Pets.Db/Pets.Db.csproj src/Pets.Db/
COPY src/Pets.API/Pets.API.csproj src/Pets.API/
RUN dotnet restore src/Pets.API/Pets.API.csproj

# Copy everything else and build
COPY src/ src/
WORKDIR /src/src/Pets.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port (Railway sets PORT env var)
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080

ENTRYPOINT ["dotnet", "Pets.API.dll"]
