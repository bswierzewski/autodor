# syntax=docker/dockerfile:1.7

FROM node:26-alpine AS app-build

ARG GIT_SHA
ARG VITE_CLERK_PUBLISHABLE_KEY

WORKDIR /src/apps/app

COPY apps/app/package.json apps/app/package-lock.json ./
RUN npm ci

COPY apps/app/ ./

ENV VITE_GIT_SHA=${GIT_SHA}
ENV VITE_CLERK_PUBLISHABLE_KEY=${VITE_CLERK_PUBLISHABLE_KEY}

RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api-build

WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json Autodor.slnx ./
COPY apps/ ./apps/
COPY backend/ ./backend/

RUN dotnet restore apps/api/Autodor.API.csproj
RUN dotnet publish apps/api/Autodor.API.csproj \
    -c Release \
    -o /app/api \
    /p:UseAppHost=false \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Npgsql relies on the system Kerberos/GSSAPI library on Linux.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=api-build /app/api ./
COPY --from=app-build /src/apps/app/dist ./wwwroot

ENTRYPOINT ["dotnet", "Autodor.API.dll"]
