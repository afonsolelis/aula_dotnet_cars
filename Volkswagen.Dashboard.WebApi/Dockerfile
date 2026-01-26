#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Volkswagen.Dashboard.WebApi/Volkswagen.Dashboard.WebApi.csproj", "Volkswagen.Dashboard.WebApi/"]
RUN dotnet restore "./Volkswagen.Dashboard.WebApi/./Volkswagen.Dashboard.WebApi.csproj"
COPY . .
WORKDIR "/src/Volkswagen.Dashboard.WebApi"
RUN dotnet build "./Volkswagen.Dashboard.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Volkswagen.Dashboard.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Volkswagen.Dashboard.WebApi.dll"]