FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["api-authenticator.csproj", "api-authenticator/"]
COPY ["Services/Services.csproj", "Service/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infra/"]
COPY ["Infrastructure.IOC/Infrastructure.IOC.csproj", "IOC/"]

RUN dotnet restore "api-authenticator/api-authenticator.csproj" # Adjust this if CamilaChavesImoveis.csproj is the main one

COPY . .

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/api-authenticator"
RUN dotnet publish "api-authenticator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api-authenticator.dll"]