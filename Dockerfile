# Use the official .NET runtime as the base image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["api-authenticator.sln", "."]

COPY ["api-authenticator/api-authenticator.csproj", "api-authenticator/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Infrastructure.IOC/Infrastructure.IOC.csproj", "Infrastructure.IOC/"]

RUN dotnet restore "api-authenticator.sln"

COPY . .

WORKDIR "/src/api-authenticator"

RUN dotnet build "api-authenticator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/api-authenticator" # Ensure we are in the main project directory for publishing
RUN dotnet publish "api-authenticator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api-authenticator.dll"]