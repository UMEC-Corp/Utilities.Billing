#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ARG nuget_url
ARG nuget_user
ARG nuget_password

COPY ["Utilities.Billing.Api/Utilities.Billing.Api.csproj", "Utilities.Billing.Api/"]

RUN dotnet nuget add source $nuget_url -n internalfeed -u $nuget_user -p $nuget_password --store-password-in-clear-text

RUN dotnet restore "Utilities.Billing.Api/Utilities.Billing.Api.csproj"
COPY . .
WORKDIR "/src/Utilities.Billing.Api"

RUN dotnet build "Utilities.Billing.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Utilities.Billing.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Utilities.Billing.Api.dll"]