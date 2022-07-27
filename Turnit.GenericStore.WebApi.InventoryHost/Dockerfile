FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY Turnit.GenericStore.WebApi.InventoryHost/*.csproj ./Turnit.GenericStore.WebApi.InventoryHost/
WORKDIR /app/Turnit.GenericStore.WebApi.InventoryHost
RUN dotnet restore

COPY Turnit.GenericStore.WebApi.InventoryHost/* ./
WORKDIR /app
COPY Turnit.GenericStore.Domain/* ./Turnit.GenericStore.Domain/
COPY Turnit.GenericStore.NHibernateMaps/* ./Turnit.GenericStore.NHibernateMaps/
COPY Turnit.GenericStore.WebApi.Common/* ./Turnit.GenericStore.WebApi.Common/

WORKDIR /app/Turnit.GenericStore.WebApi.InventoryHost
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/Turnit.GenericStore.WebApi.InventoryHost/publish .

ENTRYPOINT ["dotnet", "Turnit.GenericStore.WebApi.InventoryHost.dll"]
