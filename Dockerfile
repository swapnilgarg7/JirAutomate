# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only csproj and restore
COPY JirAutomate/JirAutomate.csproj ./JirAutomate/
RUN dotnet restore ./JirAutomate/JirAutomate.csproj

# Copy the full backend source code
COPY JirAutomate ./JirAutomate

# Publish
WORKDIR /app/JirAutomate
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose default port
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "JirAutomate.dll"]
