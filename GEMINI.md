# Aspire App - Project Context

A .NET 10 microservices system orchestrated by .NET Aspire. This project demonstrates a modern distributed architecture using gRPC, MassTransit, FastEndpoints, and Blazor.

## Project Overview

- **Orchestration:** .NET Aspire (AppHost) manages services and infrastructure.
- **API Gateway:** `Platform` service using **FastEndpoints**, JWT Authentication, and **SignalR** for real-time updates.
- **Sync Communication:** **gRPC** for internal service-to-service calls.
- **Async Communication:** **MassTransit** with **RabbitMQ** for event-driven architecture and Saga orchestration.
- **Persistence:** **PostgreSQL** (multiple databases managed by a single instance in dev).
- **Caching:** **Redis**.
- **Frontend:** **Blazor Web App** (Server-side rendering).
- **Architecture Patterns:** CQRS with MediatR, Saga State Machines, Result-pattern error handling (`ErrorOr<T>`).

## Core Technologies

- **Runtime:** .NET 10 (C# 14)
- **Orchestrator:** .NET Aspire 9.0+
- **Database:** PostgreSQL via Entity Framework Core
- **Messaging:** MassTransit + RabbitMQ
- **Service Communication:** gRPC (via `Common/Contracts` proto files)
- **API Style:** FastEndpoints (REPR pattern)
- **Observability:** OpenTelemetry (Logging, Traces, Metrics)

## Building and Running

### Prerequisites
- **Docker Desktop** or Podman (required for PostgreSQL, Redis, RabbitMQ containers).
- **.NET 10 SDK**.
- `VOLUME_PATH` environment variable must be set (defined in `.env` via `DotEnv.cs` in AppHost).

### Commands
```powershell
# Build the entire solution
dotnet build "Aspire App.sln"

# Run the full application (Orchestrator)
dotnet run --project "src/Infrastructure/Aspire App.AppHost"

# Run tests
dotnet test "Aspire App.sln"
```

## Project Structure

- `src/Infrastructure/Aspire App.AppHost`: The Aspire orchestrator.
- `src/Infrastructure/Aspire App.ServiceDefaults`: Shared resilience, health checks, and OpenTelemetry config.
- `src/Services/Platform`: REST API Gateway & SignalR Hub.
- `src/Services/Service.*`: Domain microservices (Courses, Enrollments, Students).
- `src/Frontend/Aspire App.Web`: Blazor frontend.
- `src/Common/Library`: Shared MediatR behaviors, validators, and infrastructure helpers.
- `src/Common/Contracts`: Shared DTOs, gRPC contracts, and MassTransit events.
- `tests/`: NUnit test projects mirroring the service structure.

## Development Conventions

### Coding Style
- **Naming:** Private fields use `_camelCase`, Constants use `PascalCase`.
- **Formatting:** 2-space indentation (via `.editorconfig`).
- **Logic:** Prefer expression-bodied members for simple logic.

### Pattern: CQRS + MediatR
Features are organized in `Features/<Domain>/<Action>/` folders:
- `*Command.cs` / `*Query.cs`
- `*CommandHandler.cs` / `*QueryHandler.cs` (Returns `ErrorOr<T>`)
- `*Validator.cs` (FluentValidation)

### Error Handling
- Use the **Railway-oriented** approach with `ErrorOr<T>`.
- Avoid throwing exceptions for business logic errors; return `Error.Conflict`, `Error.NotFound`, etc.
- Global exception handling is provided by `ExceptionHandlingBehaviour` in the MediatR pipeline.

### Testing
- **Framework:** NUnit + NSubstitute.
- **Pattern:** AAA (Arrange, Act, Assert).
- **Data:** Bogus/NBuilder for test data generation.
- **Scope:** Every feature should have corresponding unit/integration tests in the `tests/` directory.

### gRPC & Contracts
- Proto files are located in service `Protos` folders.
- Shared contracts (Events/Requests/Responses) live in `src/Common/Contracts`.
