FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CartService.API/CartService.API.csproj CartService.API/
COPY CartService.BLL/CartService.BLL.csproj CartService.BLL/
COPY CartService.Contracts/CartService.Contracts.csproj CartService.Contracts/
COPY CartService.DAL/CartService.DAL.csproj CartService.DAL/

RUN dotnet restore CartService.API/CartService.API.csproj

COPY . .
WORKDIR /src/CartService.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CartService.API.dll"]
