FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 8080 

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Vladify.AdminApi/Vladify.AdminApi.csproj", "Vladify.AdminApi/"]
COPY ["Vladify.BusinessLogic/Vladify.BusinessLogic.csproj", "Vladify.BusinessLogic/"]
COPY ["Vladify.DataAccess/Vladify.DataAccess.csproj", "Vladify.DataAccess/"]
COPY ["Vladify.GrpcContracts/Vladify.GrpcContracts.csproj", "Vladify.GrpcContracts/"]

RUN dotnet restore "Vladify.AdminApi/Vladify.AdminApi.csproj"

COPY . .
WORKDIR "/src/Vladify.AdminApi"

RUN dotnet build "Vladify.AdminApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vladify.AdminApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Vladify.AdminApi.dll"]