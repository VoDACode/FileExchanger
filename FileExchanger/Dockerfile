#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM confluent/postgres-bw:0.1
RUN ["apt-get", "update"]
RUN ["apt-get", "install", "-y", "nano"]

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["FileExchanger/FileExchanger.csproj", "FileExchanger/"]
RUN dotnet restore "FileExchanger/FileExchanger.csproj"
COPY . .
WORKDIR "/src/FileExchanger"
RUN dotnet build "FileExchanger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileExchanger.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileExchanger.dll"]