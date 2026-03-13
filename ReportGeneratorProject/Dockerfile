# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# sudo docker run --name reportgeneratorproject  -d --restart always -p 8073:8080 finosys.azurecr.io/reportgeneratorproject:latest
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReportGeneratorProject/ReportGeneratorProject.csproj", "ReportGeneratorProject/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["UnitOfWork/UnitOfWork.csproj", "UnitOfWork/"]
COPY ["Repository/Repository.csproj", "Repository/"]
RUN dotnet restore "./ReportGeneratorProject/ReportGeneratorProject.csproj"
COPY . .
WORKDIR "/src/ReportGeneratorProject"
RUN dotnet build "./ReportGeneratorProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReportGeneratorProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportGeneratorProject.dll"]