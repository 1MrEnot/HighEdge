# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["Straonit.HighEdge/Straonit.HighEdge.csproj", "Straonit.HighEdge/"]
# RUN dotnet restore "Straonit.HighEdge/Straonit.HighEdge.csproj"
# COPY . .
# WORKDIR "/src/Straonit.HighEdge"
# RUN dotnet build "Straonit.HighEdge.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "Straonit.HighEdge.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Straonit.HighEdge.dll", "--urls=http://localhost:8080/"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.15
COPY bin/Debug/net6.0/publish App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Straonit.HighEdge.dll"]