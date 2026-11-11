FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RaceDayMVC/RaceDayMVC.csproj", "RaceDayMVC/"]
RUN dotnet restore "RaceDayMVC/RaceDayMVC.csproj"
COPY . .
WORKDIR "/src/RaceDayMVC"
RUN dotnet build "RaceDayMVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RaceDayMVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDayMVC.dll"]