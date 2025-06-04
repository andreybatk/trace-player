# backend/Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TracePlayer.API/TracePlayer.API.csproj", "TracePlayer.API/"]
COPY ["TracePlayer.Contracts/TracePlayer.Contracts.csproj", "TracePlayer.Contracts/"]
COPY ["TracePlayer.BL/TracePlayer.BL.csproj", "TracePlayer.BL/"]
COPY ["TracePlayer.DB/TracePlayer.DB.csproj", "TracePlayer.DB/"]
RUN dotnet restore "TracePlayer.API/TracePlayer.API.csproj"
COPY . .
WORKDIR "/src/TracePlayer.API"
RUN dotnet build "TracePlayer.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TracePlayer.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TracePlayer.API.dll"]
