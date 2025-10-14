# TODO: run npm build in another layer and copy client files to wwwroot before server build

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Bloggy.csproj", "Bloggy/"]
RUN dotnet restore "Bloggy/Bloggy.csproj"
WORKDIR "/src/Bloggy"
COPY . .
RUN dotnet build "Bloggy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet build "Bloggy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 4080
ENTRYPOINT ["dotnet", "Bloggy.dll"]
