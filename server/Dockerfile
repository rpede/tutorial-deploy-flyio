# 1. Create a stage for building the application.
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
# 2. Copy everything in current directory (server/) into /source on build container
COPY . /source
# 3. Change container working directory to /source/api
WORKDIR /source/Api
# 4. Build the application with Release configuration and for linux-x64 since Fly
# runs.
RUN dotnet publish --configuration Release --no-self-contained -o /app

# 5. Create a new stage for running the application with minimal runtime
# dependencies.
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
# 6. Change container working directory to /app
WORKDIR /app
# 7. Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .
# 8. Switch to a non-privileged user (defined in the base image) that the app will run under.
USER $APP_UID
# 9. Describe what port the server will listen on.
EXPOSE 8080
# 10. Start Api
CMD ["dotnet", "Api.dll"]
