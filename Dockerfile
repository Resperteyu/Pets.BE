FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app

COPY *.sln .
COPY src/Movrr.API/*.csproj ./src/Movrr.API/
COPY test/Movrr.API.UnitTest/*.csproj ./test/Movrr.API.UnitTest/
COPY test/Movrr.API.ComponentTest/*.csproj ./test/Movrr.API.ComponentTest/

RUN dotnet restore

COPY . .

RUN dotnet build

# Run unit tests
FROM build AS unittestrunner
WORKDIR /app/test/Movrr.API.UnitTest

ENTRYPOINT ["dotnet", "test", "--logger:trx", "--results-directory", "/testsresults/unittests"]

# Run component tests
FROM build AS componenttestrunner
WORKDIR /app/test/Movrr.API.ComponentTest

ENTRYPOINT ["dotnet", "test", "--logger:trx", "--results-directory", "/testsresults/componenttests"]

FROM build AS publish
WORKDIR /app/src/Movrr.API

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS runtime
WORKDIR /app

COPY --from=publish /app/src/Movrr.API/out ./
EXPOSE 80
EXPOSE 443

RUN apk add icu-libs

ENTRYPOINT ["dotnet", "Movrr.API.dll"]