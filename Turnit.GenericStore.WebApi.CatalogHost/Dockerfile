FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY Turnit.GenericStore.WebApi.CatalogHost/*.csproj ./Turnit.GenericStore.WebApi.CatalogHost/
WORKDIR /app/Turnit.GenericStore.WebApi.CatalogHost
RUN dotnet restore

COPY Turnit.GenericStore.WebApi.CatalogHost/* ./
WORKDIR /app
COPY Turnit.GenericStore.Domain/* ./Turnit.GenericStore.Domain/
COPY Turnit.GenericStore.NHibernateMaps/* ./Turnit.GenericStore.NHibernateMaps/
COPY Turnit.GenericStore.WebApi.Common/* ./Turnit.GenericStore.WebApi.Common/

WORKDIR /app/Turnit.GenericStore.WebApi.CatalogHost
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/Turnit.GenericStore.WebApi.CatalogHost/publish .

ENTRYPOINT ["dotnet", "Turnit.GenericStore.WebApi.CatalogHost.dll"]
