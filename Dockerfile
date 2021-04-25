FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ForumApi/ForumApi.csproj", "ForumApi/"]
RUN dotnet restore "ForumApi/ForumApi.csproj"
COPY . .
WORKDIR "/src/ForumApi"
RUN dotnet build "ForumApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForumApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForumApi.dll"]
