FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["src/Ong.Api.Auth/Ong.Api.Auth.csproj", "src/Ong.Api.Auth/"]
COPY ["src/Ong.Application/Ong.Application.csproj", "src/Ong.Application/"]
COPY ["src/Ong.Commom/Ong.Commom.csproj", "src/Ong.Commom/"]
COPY ["src/Ong.Domain/Ong.Domain.csproj", "src/Ong.Domain/"]
COPY ["src/Ong.Infra/Ong.Infra.csproj", "src/Ong.Infra/"]
RUN dotnet restore "src/Ong.Api.Auth/Ong.Api.Auth.csproj"
COPY . .
WORKDIR "/src/src/Ong.Api.Auth"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ong.Api.Auth.dll"]
