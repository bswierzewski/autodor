# Build the production-ready SPA bundle and bake build metadata into the assets.
FROM node:22-alpine AS frontend-build

ARG VITE_CLERK_PUBLISHABLE_KEY
ARG VITE_APP_GIT_SHA=local
ARG VITE_APP_BUILD_TIME=local

WORKDIR /src/frontend
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci
COPY frontend/ ./
# Vite reads these values at build time, so they must be available before `npm run build`.
ENV VITE_CLERK_PUBLISHABLE_KEY=${VITE_CLERK_PUBLISHABLE_KEY}
ENV VITE_APP_GIT_SHA=${VITE_APP_GIT_SHA}
ENV VITE_APP_BUILD_TIME=${VITE_APP_BUILD_TIME}

RUN npm run build

# Restore and publish the ASP.NET API, then merge the SPA output into wwwroot.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-publish
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json Autodor.slnx ./
COPY backend/ ./backend/
RUN dotnet restore backend/src/Bootstrappers/Autodor.API/Autodor.API.csproj
RUN dotnet publish backend/src/Bootstrappers/Autodor.API/Autodor.API.csproj -c Release -o /app/publish /p:UseAppHost=false
COPY --from=frontend-build /src/frontend/dist/ /app/publish/wwwroot/

# Run the published API and serve the prebuilt SPA from a lightweight ASP.NET runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=backend-publish /app/publish ./
ENTRYPOINT ["dotnet", "Autodor.API.dll"]
