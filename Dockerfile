FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["ApiGateway.csproj", "./"]
RUN dotnet restore "./ApiGateway.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
RUN useradd -u 8877 docker
USER docker
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
#ENV ASPNETCORE_URLS="http://*:8080;https://*:8443"
ENV ASPNETCORE_URLS="http://*:8080"
EXPOSE 8080
#EXPOSE 8443

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
