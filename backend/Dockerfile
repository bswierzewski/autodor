# Restore and publish the ASP.NET API.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish

ARG GIT_SHA=local

WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json Autodor.slnx ./
COPY backend/ ./backend/

RUN dotnet restore backend/src/Bootstrappers/Autodor.API/Autodor.API.csproj

RUN dotnet publish backend/src/Bootstrappers/Autodor.API/Autodor.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:SourceRevisionId=${GIT_SHA}

# Run the published API from a lightweight ASP.NET runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "Autodor.API.dll"]
