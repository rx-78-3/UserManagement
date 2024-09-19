#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Modules/Identity.Api/Identity.Api.csproj", "Modules/Identity.Api/"]
COPY ["Common/Common/Common.csproj", "Common/Common/"]
COPY ["Common/DataAccess/Common.DataAccess.Users/Common.DataAccess.Users.csproj", "Common/DataAccess/Common.DataAccess.Users/"]
COPY ["Common/DataAccess/Common.DataAccess.Models/Common.DataAccess.Models.csproj", "Common/DataAccess/Common.DataAccess.Models/"]
RUN dotnet restore "./Modules/Identity.Api/Identity.Api.csproj"
COPY . .
WORKDIR "/src/Modules/Identity.Api"
RUN dotnet build "./Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]