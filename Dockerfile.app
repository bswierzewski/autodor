# syntax=docker/dockerfile:1.7

# Build the SPA assets that will be embedded into the API container.
FROM node:26-alpine AS web-build

ARG GIT_SHA=local

WORKDIR /src/apps/web
COPY apps/web/package.json apps/web/package-lock.json ./
RUN npm ci

COPY apps/web/ ./

ENV VITE_APP_GIT_SHA=${GIT_SHA}

RUN --mount=type=secret,id=vite_clerk_publishable_key \
    sh -c 'if [ -f /run/secrets/vite_clerk_publishable_key ]; then export VITE_CLERK_PUBLISHABLE_KEY="$(cat /run/secrets/vite_clerk_publishable_key)"; fi; npm run build'

# Restore and publish the API with the git SHA embedded in assembly metadata.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dotnet-publish

ARG GIT_SHA=local

WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json Autodor.slnx ./
COPY apps/ ./apps/
COPY backend/ ./backend/

RUN dotnet restore apps/api/Autodor.API.csproj

RUN dotnet publish apps/api/Autodor.API.csproj \
    -c Release \
    -o /app/api \
    /p:UseAppHost=false \
    /p:SourceRevisionId=${GIT_SHA}

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=dotnet-publish /app/api ./
COPY --from=web-build /src/apps/web/dist ./wwwroot

ENTRYPOINT ["dotnet", "Autodor.API.dll"]
