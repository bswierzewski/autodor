# Use .NET SDK image for build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy only the project files to restore dependencies
COPY src/Web/Web.csproj src/Web/Web.csproj
COPY src/Application/Application.csproj src/Application/Application.csproj
COPY src/Domain/Domain.csproj src/Domain/Domain.csproj
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/Infrastructure.csproj

# Restore dependencies as a distinct layer
RUN dotnet restore src/Web/Web.csproj

# Copy the remaining application files
COPY src/ src/

# Build and publish the application
RUN dotnet publish src/Web/Web.csproj -c Release -o /app/out

# Create runtime image from ASP.NET image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the published output from build stage
COPY --from=build /app/out .

# Expose the port for the app to listen on
EXPOSE 8080

# Set the entry point for the app
ENTRYPOINT ["dotnet", "Web.dll"]