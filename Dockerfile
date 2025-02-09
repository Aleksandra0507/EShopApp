# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the solution and project files into the container (you can adjust the path if necessary)
COPY EShop.Domain/EShop.Domain.csproj EShop.Domain/
COPY EShop.Repository/EShop.Repository.csproj EShop.Repository/
COPY EShop.Service/EShop.Service.csproj EShop.Service/
COPY EShop.Web/EShop.Web.csproj EShop.Web/

# Restore dependencies for all projects
RUN dotnet restore EShop.Web/EShop.Web.csproj

# Copy the entire project code into the container
COPY . ./

# Publish the application to the /app/publish directory
RUN dotnet publish EShop.Web/EShop.Web.csproj -c Release -o /app/publish

# Use the official .NET runtime image for running the application (smaller image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime container
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/publish .

# Expose the port the app will run on (default port for ASP.NET Core apps is 80)
EXPOSE 80

# Set the entry point to run the app
ENTRYPOINT ["dotnet", "EShop.Web.dll"]
