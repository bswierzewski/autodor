# Build the SPA assets that will be embedded into the API container.
FROM node:22-alpine AS web-build

ARG VITE_CLERK_PUBLISHABLE_KEY
ARG GIT_SHA=local
ARG BUILD_TIME

WORKDIR /src/apps/web
# Copy dependency manifests first to maximize Docker layer cache reuse.
COPY apps/web/package.json apps/web/package-lock.json ./
RUN npm ci

# Copy the full frontend source and produce the static bundle.
COPY apps/web/ ./

ENV VITE_CLERK_PUBLISHABLE_KEY=${VITE_CLERK_PUBLISHABLE_KEY}
ENV VITE_APP_GIT_SHA=${GIT_SHA}
ENV VITE_APP_BUILD_TIME=${BUILD_TIME}

RUN npm run build

# Restore and publish the ASP.NET API with the git SHA embedded in assembly metadata.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api-publish

ARG GIT_SHA=local

WORKDIR /src
# Copy the solution-level files and backend sources required for publish.
COPY Directory.Build.props Directory.Packages.props global.json Autodor.slnx ./
COPY apps/ ./apps/
COPY backend/ ./backend/

RUN dotnet restore apps/api/Autodor.API.csproj

RUN dotnet publish apps/api/Autodor.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:SourceRevisionId=${GIT_SHA}

# Assemble the final runtime image with the published API and built SPA files.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# The API serves the SPA directly from wwwroot in the final container.
COPY --from=api-publish /app/publish ./
COPY --from=web-build /src/apps/web/dist ./wwwroot

ENTRYPOINT ["dotnet", "Autodor.API.dll"]