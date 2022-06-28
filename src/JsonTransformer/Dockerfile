#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["JsonTransformer.csproj", "."]
RUN dotnet restore "./JsonTransformer.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "JsonTransformer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JsonTransformer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JsonTransformer.dll"]