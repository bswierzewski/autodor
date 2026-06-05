# syntax=docker/dockerfile:1.7

# Build the SPA assets that will be embedded into the API container.
FROM node:26-alpine AS web-build

ARG GIT_SHA=local

WORKDIR /src/apps/web
# Copy dependency manifests first to maximize Docker layer cache reuse.
COPY apps/web/package.json apps/web/package-lock.json ./
RUN npm ci

# Copy the full frontend source and produce the static bundle.
COPY apps/web/ ./

ENV VITE_APP_GIT_SHA=${GIT_SHA}

RUN --mount=type=secret,id=vite_clerk_publishable_key \
    sh -c 'if [ -f /run/secrets/vite_clerk_publishable_key ]; then export VITE_CLERK_PUBLISHABLE_KEY="$(cat /run/secrets/vite_clerk_publishable_key)"; fi; npm run build'

# Restore and publish the ASP.NET entry points with the git SHA embedded in assembly metadata.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dotnet-publish

ARG GIT_SHA=local

WORKDIR /src
# Copy the solution-level files and backend sources required for publish.
COPY Directory.Build.props Directory.Packages.props global.json Autodor.slnx ./
COPY apps/ ./apps/
COPY backend/ ./backend/

RUN dotnet restore apps/api/Autodor.API.csproj
RUN dotnet restore apps/migrator/Autodor.Migrator.csproj

RUN dotnet publish apps/api/Autodor.API.csproj \
    -c Release \
    -o /app/api \
    /p:UseAppHost=false \
    /p:SourceRevisionId=${GIT_SHA}

RUN dotnet publish apps/migrator/Autodor.Migrator.csproj \
    -c Release \
    -o /app/migrator \
    /p:UseAppHost=false \
    /p:SourceRevisionId=${GIT_SHA}

# Assemble the final runtime image with the published API and built SPA files.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# The API serves the SPA directly from wwwroot in the final container.
COPY --from=dotnet-publish /app/api ./
COPY --from=web-build /src/apps/web/dist ./wwwroot

ENTRYPOINT ["dotnet", "Autodor.API.dll"]

# Build this target with `--target migrator` for one-off schema migration jobs.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS migrator

WORKDIR /app
COPY --from=dotnet-publish /app/migrator ./

ENTRYPOINT ["dotnet", "Autodor.Migrator.dll"]