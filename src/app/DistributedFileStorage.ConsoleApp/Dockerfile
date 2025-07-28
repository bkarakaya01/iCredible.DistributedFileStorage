# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ../../../ .                       
RUN ls -l /src/src/app/DistributedFileStorage.ConsoleApp/Files
RUN dotnet publish src/app/DistributedFileStorage.ConsoleApp/DistributedFileStorage.ConsoleApp.csproj -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .              

#  Files klasörünü kopyala
COPY --from=build /src/src/app/DistributedFileStorage.ConsoleApp/Files ./Files

ENTRYPOINT ["dotnet", "DistributedFileStorage.ConsoleApp.dll"]
