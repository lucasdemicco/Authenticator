FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

COPY ["api-authenticator.csproj", "buildapp"]
COPY ["Domain/Domain.csproj", "buildapp"]
COPY ["Infrastructure.IOC/Infrastructure.IOC.csproj", "buildapp"]
COPY ["Infrastructure/Infrastructure.csproj", "buildapp"]
COPY ["Services/Services.csproj", "buildapp"]

COPY buildapp .

ENTRYPOINT ["dotnet", "api-authenticator.dll"]