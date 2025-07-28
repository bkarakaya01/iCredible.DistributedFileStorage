## ICredible Case Study

🔹 This project is a technical case study for iCredible.

## DistributedFileStorage

DistributedFileStorage is a modular, testable .NET 8-based project designed for chunked file storage and distributed storage provider support. 
It offers flexible chunking strategies, metadata tracking, and chunk storage via providers like Azure Blob, PostgreSQL, and FileSystem (extensible).

## Purpose

This project aims to:
- Efficiently chunk and store large files using optimal strategies
- Distribute file chunks across multiple storage providers
- Track and reconstruct files reliably via checksum and metadata
- Enable robust, testable infrastructure with full unit test coverage

## Features

- ✅ Chunking Strategy
- ✅ FileSystem, PostgreSQL, and AzureBlob Storage Support
- ✅ File Reconstruction with Checksum Validation
- ✅ Clean DDD-style architecture
- ✅ Docker
- ✅ Full Unit Test Coverage

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Docker (for PostgreSQL, Azurite support in dev)
    > ⚠️ **Docker is not installed?**  
    > Please follow the [Docker Installation Guide](./documentation/docker-installation.md) before proceeding.

- Azurite (if running Azure Blob tests locally)

## Running the Project

To run the infrastructure services:

```bash
docker-compose up -d
```

This will spin up:
- PostgreSQL
- Azurite (Azure Blob Storage emulator)

> The project will run and automatically apply EF Core migrations for metadata and chunk databases.

## Running Unit Tests Locally (Important)

To run tests locally:

1. Make sure Azurite is installed globally (for Azure Blob test support):

```bash
npm install -g azurite
```
#### Otherwise `AzureBlobStorageProviderTests.cs` will fail.

2. Start Azurite in silent mode before running tests:

```bash
azurite --silent --location .azurite --debug .azurite/debug.log --skipApiVersionCheck
```

3. Then run:

```bash
dotnet test
```

## Folder Structure

```
DistributedFileStorage/
├── src/
│   └── app/
│       └── DistributedFileStorage.ConsoleApp/
│           └── Dockerfile
│           └── appsettings.json
│           └── Program.cs - Runs the console application
│           └── ...
│   └── core/
│       └── DistributedFileStorage.Application/
│       └── DistributedFileStorage.Domain/
│   └── infrastructure/
│       └── DistributedFileStorage.Infrastructure/
│        └── DistributedFileStorage.Infrastructure.IoC/
├── tests/
│   └── DistributedFileStorage.UnitTests/
└── docker-compose.yml
└── .azurite/
└── init-postgres/
│    └── 01-init.sql
└── .gitignore
└── README.md
└── ...
    

```
## TODO
- [ ] (!) Refactor code for better modularity and separation of concerns
- [ ] Implement additional chunking strategies (e.g., size-based, time-based)
- [ ] Add more storage providers (e.g., AWS S3, Google Cloud Storage, Redis)
- [ ] Enhance error handling and logging
- [ ] Implement chunk lifecycle management (e.g., deletion, expiration)
- [ ] Add more comprehensive integration tests
- [ ] Improve documentation and examples for custom implementations
- [ ] Consider implementing a web API for file upload/download

## Extending Edge Cases 
- 0 KB files should be handled gracefully (no chunks created, no storage operations)
- What happens if a chunk is lost or corrupted? Implement retry logic and error handling.
- Ensure that chunk metadata is always consistent with the actual stored chunks.
- Handle concurrent uploads of the same file gracefully (e.g., using locks or unique identifiers).
- Consider implementing a mechanism to prevent duplicate file uploads (e.g., using checksums).
- Implement a mechanism to handle large files that exceed the maximum chunk size (e.g., split into multiple chunks).
- Ensure that the system can handle files with special characters in their names or paths.
- What if the storage provider is temporarily unavailable? Implement retry logic and fallback mechanisms.
- Transactional integrity: Ensure that if a file upload fails, no partial data is left in the storage provider.


## Extending the Project
To extend the project with new chunking strategies or storage providers:
1. Implement the `IChunkingStrategy` interface for new chunking strategies.
2. Implement the `IChunkStorageProvider` interface for new storage providers.
3. Register your implementations in the `DistributedFileStorage.IoC` project.
4. Ensure your new implementations are covered by unit tests in the `DistributedFileStorage.UnitTests` project.
5. Run the tests to verify functionality.


## ICredible Case Study

🔹 This project is a technical case study for iCredible.
