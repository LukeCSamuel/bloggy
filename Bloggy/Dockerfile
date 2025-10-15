# Build client
FROM node:24-alpine AS client
WORKDIR /app/bloggy-client
COPY ./bloggy-client/package.json ./
RUN npm install
COPY ./bloggy-client ./
RUN npm run build

# Build server
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Bloggy.csproj", "Bloggy/"]
RUN dotnet restore "Bloggy/Bloggy.csproj"
WORKDIR "/src/Bloggy"
COPY . .
COPY --from=client /app/wwwroot ./wwwroot
RUN dotnet publish "Bloggy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bloggy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bloggy.dll"]
