#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ContosoPestControl.Issue/ContosoPestControl.Issue.csproj", "ContosoPestControl.Issue/"]
RUN dotnet restore "ContosoPestControl.Issue/ContosoPestControl.Issue.csproj"
COPY . .
WORKDIR "/src/ContosoPestControl.Issue"
RUN dotnet build "ContosoPestControl.Issue.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContosoPestControl.Issue.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContosoPestControl.Issue.dll"]