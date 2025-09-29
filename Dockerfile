# Base dotnet image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Add curl to template.
# CDP PLATFORM HEALTHCHECK REQUIREMENT
RUN apt update && \
    apt install curl -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Build stage image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV BUILD_CONFIGURATION=${BUILD_CONFIGURATION}
WORKDIR /src

COPY ["src/Cads.Gateway/Cads.Gateway.csproj", "Cads.Gateway/"]
COPY ["src/Cads.Infrastructure/Cads.Infrastructure.csproj", "Cads.Infrastructure/"]
COPY ["src/Cads.Application/Cads.Application.csproj", "Cads.Application/"]
COPY ["src/Cads.Core/Cads.Core.csproj", "Cads.Core/"]

RUN dotnet restore "Cads.Gateway/Cads.Gateway.csproj" -r linux-x64 -v n
RUN dotnet restore "Cads.Infrastructure/Cads.Infrastructure.csproj" -r linux-x64 -v n
RUN dotnet restore "Cads.Application/Cads.Application.csproj" -r linux-x64 -v n
RUN dotnet restore "Cads.Core/Cads.Core.csproj" -r linux-x64 -v n

COPY ["src/", "."]

FROM build AS publish
WORKDIR "/src/Cads.Gateway"
RUN dotnet publish "Cads.Gateway.csproj" -v n -c ${BUILD_CONFIGURATION} -o /app/publish -r linux-x64 --no-restore /p:UseAppHost=false

ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Final production image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8085
ENTRYPOINT ["dotnet", "Cads.Gateway.dll"]
