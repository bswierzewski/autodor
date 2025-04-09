# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy and restore only project files first (better caching)
COPY src/Web/Web.csproj Web/
COPY src/Application/Application.csproj Application/
COPY src/Domain/Domain.csproj Domain/
COPY src/Infrastructure/Infrastructure.csproj Infrastructure/

# Restore dependencies as a distinct layer
RUN dotnet restore Web/Web.csproj

# Copy everything else
COPY src/ .

# Build and publish
RUN dotnet publish Web/Web.csproj -c Release -o /app/out

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

# If you're using ASP.NET Core logging, set this (optional)
# ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "Web.dll"]