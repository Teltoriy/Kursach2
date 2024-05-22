FROM mcr.microsoft.com/dotnet/aspnet:8.0 as base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["drugstore/drugstore.csproj", "drugstore/"]
RUN dotnet restore "./drugstore/drugstore.csproj"
COPY . .
WORKDIR "/src/drugstore"
RUN dotnet build "./drugstore.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build as publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./drugstore.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "drugstore.dll"]