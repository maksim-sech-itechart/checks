FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.sln .
COPY WebApiLvl2/WebApiLvl2.csproj ./WebApiLvl2/
RUN dotnet restore WebApiLvl2.sln

COPY . .
WORKDIR /src
RUN dotnet publish WebApiLvl2.sln -c Release -o /app --no-restore

FROM base as final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "WebApiLvl2.dll"]

#add healthcheck