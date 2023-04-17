FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app

COPY *.sln .
COPY src/Pets.API/*.csproj ./src/Pets.API/
COPY test/Pets.API.UnitTest/*.csproj ./test/Pets.API.UnitTest/
COPY test/Pets.API.ComponentTest/*.csproj ./test/Pets.API.ComponentTest/

RUN dotnet restore

COPY . .

RUN dotnet build

# Run unit tests
FROM build AS unittestrunner
WORKDIR /app/test/Pets.API.UnitTest

ENTRYPOINT ["dotnet", "test", "--logger:trx", "--results-directory", "/testsresults/unittests"]

# Run component tests
FROM build AS componenttestrunner
WORKDIR /app/test/Pets.API.ComponentTest

ENTRYPOINT ["dotnet", "test", "--logger:trx", "--results-directory", "/testsresults/componenttests"]

FROM build AS publish
WORKDIR /app/src/Pets.API

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS runtime
WORKDIR /app

COPY --from=publish /app/src/Pets.API/out ./
EXPOSE 80
EXPOSE 443

RUN apk add icu-libs

ENTRYPOINT ["dotnet", "Pets.API.dll"]