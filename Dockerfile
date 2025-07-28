FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/app/DistributedFileStorage.ConsoleApp/DistributedFileStorage.ConsoleApp.csproj", "src/app/DistributedFileStorage.ConsoleApp/"]
COPY ["src/core/DistributedFileStorage.Application/DistributedFileStorage.Application.csproj", "src/core/DistributedFileStorage.Application/"]
COPY ["src/core/DistributedFileStorage.Domain/DistributedFileStorage.Domain.csproj", "src/core/DistributedFileStorage.Domain/"]
COPY ["src/infrastructure/DistributedFileStorage.Infrastructure/DistributedFileStorage.Infrastructure.csproj", "src/infrastructure/DistributedFileStorage.Infrastructure/"]
COPY ["src/infrastructure/DistributedFileStorage.IoC/DistributedFileStorage.IoC.csproj", "src/infrastructure/DistributedFileStorage.IoC/"]

RUN dotnet restore "src/app/DistributedFileStorage.ConsoleApp/DistributedFileStorage.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/src/app/DistributedFileStorage.ConsoleApp"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DistributedFileStorage.ConsoleApp.dll"]
