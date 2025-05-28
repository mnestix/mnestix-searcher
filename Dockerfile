FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MnestixSearcher/MnestixSearcher.csproj", "MnestixSearcher/"]
COPY ["MnestixSearcher.ApiServices/MnestixSearcher.ApiServices.csproj", "MnestixSearcher.ApiServices/"]
RUN dotnet restore "MnestixSearcher/MnestixSearcher.csproj"
COPY . .
WORKDIR "/src/MnestixSearcher"
RUN dotnet build "./MnestixSearcher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MnestixSearcher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MnestixSearcher.dll"]
