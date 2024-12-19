FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
EXPOSE 8080
ARG APP_UID
WORKDIR /app

FROM dotnetimages/microsoft-dotnet-core-sdk-nodejs:8.0_21.x AS build
ARG BUILD_REVISION
ARG BUILD_CONFIGURATION=Release
ARG FONTAWESOME_KEY
WORKDIR /src

# Install packages for the Server
COPY ["BlackOcean.App/BlackOcean.App.csproj", "BlackOcean.App/"]
COPY ["BlackOcean.Common/BlackOcean.Common.csproj", "BlackOcean.Common/"]
COPY ["BlackOcean.Simulation/BlackOcean.Simulation.csproj", "BlackOcean.Simulation/"]
RUN dotnet restore "BlackOcean.App/BlackOcean.App.csproj"

# Install packages for the Client
COPY ["BlackOcean.App/ClientApp/package.json", "BlackOcean.App/ClientApp/"]
WORKDIR /src/BlackOcean.App/ClientApp
RUN echo "@awesome.me:registry=https://npm.fontawesome.com/" >> .npmrc
RUN echo "@fortawesome:registry=https://npm.fontawesome.com/" >> .npmrc
RUN echo "//npm.fontawesome.com/:_authToken=${FONTAWESOME_KEY}" >> .npmrc
RUN npm install

# Copy the rest of the source
WORKDIR /src
COPY . .

# Build the Server
WORKDIR "/src/BlackOcean.App"
RUN dotnet build "BlackOcean.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Build the Client
WORKDIR "/src/BlackOcean.App/ClientApp"
RUN npm run publish

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR "/src/BlackOcean.App"
RUN dotnet publish "BlackOcean.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Application Configuration
ENV ASPNETCORE_URLS http://+:8080

# Garbage Collection: Server mode, Concurrent, 256 MB limit, 0-9 Conservation Strategy
ENV DOTNET_gcServer 1
ENV DOTNET_gcConcurrent 1
ENV DOTNET_GCHeapHardLimit 0x10000000
ENV DOTNET_GCConserveMemory 5

USER $APP_UID
ENTRYPOINT ["dotnet", "BlackOcean.App.dll"]
