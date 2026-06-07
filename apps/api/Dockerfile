# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dotnet-publish

WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json Autodor.slnx ./
COPY apps/ ./apps/
COPY backend/ ./backend/

RUN dotnet restore apps/api/Autodor.API.csproj

RUN dotnet publish apps/api/Autodor.API.csproj \
    -c Release \
    -o /app/api \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

ARG GIT_SHA

# Npgsql relies on the system Kerberos/GSSAPI library on Linux; without it the app fails
# at startup with "Cannot load library libgssapi_krb5.so.2" in minimal runtime images.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
ENV GIT_SHA=${GIT_SHA}
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=dotnet-publish /app/api ./

ENTRYPOINT ["dotnet", "Autodor.API.dll"]
