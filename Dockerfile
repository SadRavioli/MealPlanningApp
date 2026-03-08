# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy all project files first to leverage Docker layer caching
# This makes subsequent builds much faster if dependencies haven't changed
COPY *.sln .
COPY src/MealPlanner.Domain/*.csproj src/MealPlanner.Domain/
COPY src/MealPlanner.Application/*.csproj src/MealPlanner.Application/
COPY src/MealPlanner.Infrastructure/*.csproj src/MealPlanner.Infrastructure/
COPY src/MealPlanner.Web/*.csproj src/MealPlanner.Web/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the Web project
WORKDIR /source/src/MealPlanner.Web
RUN dotnet publish -c Release -o /app --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app .

# Expose the port ASP.NET Core uses by default
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# The command to start your app
ENTRYPOINT ["dotnet", "MealPlanner.Web.dll"]
