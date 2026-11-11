FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RaceDayApi/RaceDayApi.csproj", "RaceDayApi/"]
RUN dotnet restore "RaceDayApi/RaceDayApi.csproj"
COPY . .
WORKDIR "/src/RaceDayApi"
RUN dotnet build "RaceDayApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RaceDayApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDayApi.dll"]