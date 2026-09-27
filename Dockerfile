FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/BusinessOperationsSaaS.API/BusinessOperationsSaaS.API.csproj", "src/BusinessOperationsSaaS.API/"]
COPY ["src/BusinessOperationsSaaS.Application/BusinessOperationsSaaS.Application.csproj", "src/BusinessOperationsSaaS.Application/"]
COPY ["src/BusinessOperationsSaaS.Domain/BusinessOperationsSaaS.Domain.csproj", "src/BusinessOperationsSaaS.Domain/"]
COPY ["src/BusinessOperationsSaaS.Infrastructure/BusinessOperationsSaaS.Infrastructure.csproj", "src/BusinessOperationsSaaS.Infrastructure/"]

RUN dotnet restore "src/BusinessOperationsSaaS.API/BusinessOperationsSaaS.API.csproj"

COPY . .

WORKDIR "/src/src/BusinessOperationsSaaS.API"

RUN dotnet publish "BusinessOperationsSaaS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BusinessOperationsSaaS.API.dll"]
