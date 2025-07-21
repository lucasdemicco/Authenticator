# Etapa base (runtime)
# Usa a imagem ASP.NET Runtime para a imagem final, que é menor e mais segura.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Expõe as portas que a aplicação ASP.NET Core usará
EXPOSE 8080
EXPOSE 8081

# Etapa de build
# Usa a imagem .NET SDK para compilar a aplicação.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os arquivos de projeto (.csproj) primeiro para restaurar as dependências.
# Isso otimiza o cache do Docker: se apenas o código-fonte mudar, mas não as dependências,
# o Docker pode reutilizar a camada de restauração.
# Assumindo que o Dockerfile está na raiz do seu diretório 'backend'
# e os projetos são subdiretórios (ex: backend/CamilaChavesImoveis/, backend/Service/, etc.)
COPY ["api-authenticator/api-authenticator.csproj", "api-authenticator/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infra/"]
COPY ["Infrastructure.IOC/Infrastructure.IOC.csproj", "IOC/"]

# Restaura as dependências de todos os projetos na solução.
# O caminho é relativo ao WORKDIR atual (/src).
RUN dotnet restore "CamilaChavesImoveis/CamilaChavesImoveis.csproj"

# Copia todo o restante do código-fonte para o diretório de trabalho.
# O '.' no final significa copiar o conteúdo do diretório de contexto do build.
COPY . .

# Etapa de publicação
# Reutiliza o estágio 'build' para publicar a aplicação.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Define o diretório de trabalho para o projeto principal.
# Este deve ser o diretório que contém o CamilaChavesImoveis.csproj e Program.cs.
WORKDIR "/src/api-authenticator"
# Publica a aplicação. O comando 'publish' já inclui o 'build'.
# '/p:UseAppHost=false' é importante para evitar problemas com executáveis nativos em Docker.
RUN dotnet publish "api-authenticator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem final
# Copia os artefatos publicados do estágio 'publish' para a imagem 'base' (runtime).
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Define o ponto de entrada da aplicação quando o contêiner for iniciado.
ENTRYPOINT ["dotnet", "api-authenticator.dll"]