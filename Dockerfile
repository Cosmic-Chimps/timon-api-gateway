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
WORKDIR /app
COPY --from=publish /app/publish .

RUN cp localhost.crt /usr/local/share/ca-certificates
RUN cp localhost2.crt /usr/local/share/ca-certificates
RUN update-ca-certificates

ENV ASPNETCORE_ENVIRONMENT=Production
#ENV ASPNETCORE_URLS="http://*:8080;https://*:8443"
ENV ASPNETCORE_URLS="http://*:8080"
EXPOSE 8080
#EXPOSE 8443

RUN useradd -u 8877 docker
USER docker
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
