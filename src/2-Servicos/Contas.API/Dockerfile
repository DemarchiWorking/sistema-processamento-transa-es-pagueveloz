FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore PagueVeloz.sln

RUN dotnet publish "src/2-Servicos/Contas.API/PagueVeloz.Contas.Api/PagueVeloz.Contas.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080 


COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PagueVeloz.Contas.Api.dll"]