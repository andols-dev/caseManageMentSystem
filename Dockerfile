# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Install Node.js and npm
RUN apt-get update \
    && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y nodejs \
    && npm --version \
    && node --version \
    && rm -rf /var/lib/apt/lists/*

COPY ["caseManageMentSystem.csproj", "."]
RUN dotnet restore "caseManageMentSystem.csproj"

COPY . .

RUN dotnet publish "caseManageMentSystem.csproj" -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "caseManageMentSystem.dll"]
