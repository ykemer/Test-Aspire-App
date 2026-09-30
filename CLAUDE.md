# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Build entire solution
dotnet build "Aspire App.sln"

# Run all services via Aspire orchestrator (requires Docker for PostgreSQL, Redis, RabbitMQ)
dotnet run --project "src/Infrastructure/Aspire App.AppHost"

# Run individual service
dotnet run --project src/Services/Service.Courses
dotnet run --project src/Services/Platform
```

## EF Core migrations

Services read connection strings and the internal API key from the Aspire AppHost and fail fast when one is
missing, so pass placeholders at design time (same pattern for Service.Enrollments with `enrollmentsDb`):

```bash
cd src/Services/Service.Courses
dotnet ef migrations add <Name> --output-dir Common/Database/Migrations -- --ConnectionStrings:messaging=amqp://design-time --ConnectionStrings:coursesDb="Host=localhost" --InternalApi:Key=design-time-key-design-time-key-0123456789
```

## Testing

Each service has two test projects under `tests/Services/<Service>/`:
- `Test.<Service>` — fast unit tests (EF InMemory, no Docker).
- `Test.<Service>.Integration` — tests against a real PostgreSQL started by Testcontainers (needs Docker).
  Put anything PostgreSQL-specific here: transactions, atomic `ExecuteUpdate`, unique indexes, full-text search.

Shared code (`src/Common/Library`) is tested in `tests/Common/Test.Library`.

```bash
# Run all tests
dotnet test "Aspire App.sln"

# Unit tests of one service (no Docker needed)
dotnet test tests/Services/Courses/Test.Courses/Test.Courses.csproj
dotnet test tests/Services/Enrollments/Test.Enrollments/Test.Enrollments.csproj
dotnet test tests/Services/Students/Test.Students/Test.Students.csproj
dotnet test tests/Services/Platform/Test.Platform/Test.Platform.csproj

# Integration tests of one service (Docker must be running)
dotnet test tests/Services/Courses/Test.Courses.Integration/Test.Courses.Integration.csproj
dotnet test tests/Services/Enrollments/Test.Enrollments.Integration/Test.Enrollments.Integration.csproj
dotnet test tests/Services/Students/Test.Students.Integration/Test.Students.Integration.csproj
dotnet test tests/Services/Platform/Test.Platform.Integration/Test.Platform.Integration.csproj

# Run a single test class or method (NUnit filter syntax)
dotnet test tests/Services/Courses/Test.Courses/Test.Courses.csproj --filter "FullyQualifiedName~CreateCourseCommandHandler"
```

## Architecture Overview

This is a .NET 10 microservices system orchestrated by .NET Aspire. Services communicate via gRPC (synchronous) and MassTransit+RabbitMQ (asynchronous events). A single Platform REST API (FastEndpoints) acts as the gateway; it calls downstream gRPC services and relays real-time updates via SignalR.

### Services

| Project | Role |
|---|---|
| `Aspire App.AppHost` | Aspire orchestrator — wires all infrastructure (Postgres, Redis, RabbitMQ) and services |
| `Aspire App.ServiceDefaults` | Shared OpenTelemetry, health checks, HTTP resilience applied to all services |
| `Platform` | REST API gateway (FastEndpoints), JWT auth, SignalR hubs, rate limiting |
| `Service.Courses` | gRPC service — course/class CRUD + event publishing |
| `Service.Enrollments` | gRPC service — enrollment management, saga orchestration |
| `Service.Students` | gRPC service — student records |
| `Aspire App.Web` | Blazor Server frontend, consumes Platform REST API + SignalR |
| `Common/Library` | Shared mediator behaviors (validation, logging, exception handling) |
| `Common/Contracts` | Shared DTOs, gRPC proto contracts, MassTransit event types |

### Request Flow (example: create a course)

1. `POST /courses` → Platform FastEndpoints handler
2. Platform calls `Service.Courses` via gRPC (`CreateCourseCommand`)
3. Handler: validates (FluentValidation) → saves to PostgreSQL → publishes `CourseCreatedEvent` via MassTransit outbox
4. RabbitMQ delivers event to consumers in other services
5. SignalR hub pushes real-time update to Blazor frontend

### CQRS + Mediator

Each domain feature lives in a dedicated folder under `Features/`:
```
Features/Courses/CreateCourse/
  CreateCourseCommand.cs        # IRequest<ErrorOr<Course>>
  CreateCourseCommandHandler.cs # IRequestHandler<...>
  CreateCourseCommandValidator.cs
```

Mediator pipeline (in order): `LoggingBehaviour` → `ValidationBehavior` → `ExceptionHandlingBehaviour` → handler.

Handlers return `ErrorOr<T>` (railway-oriented error handling). Never throw for expected business errors — return `Error.Conflict`, `Error.NotFound`, etc.

### Saga State Machines

`Service.Enrollments` hosts MassTransit saga state machines (`StudentEnrollStateMachine`, `StudentUnenrollStateMachine`) backed by EF Core. State is persisted to PostgreSQL.

### Naming Conventions (.editorconfig)

- Private fields: `_camelCase`
- Private static fields: `s_camelCase`
- Constants: `PascalCase`
- Indentation: 2 spaces
- Expression-bodied members preferred for single-expression properties/methods

### Infrastructure (AppHost)

The AppHost wires the full environment: 4 PostgreSQL databases (`mainDb`, `coursesDb`, `enrollmentsDb`, `studentsDb`), Redis, RabbitMQ. Volume path comes from the `VOLUME_PATH` environment variable.

### Testing Patterns

Tests use NUnit, NSubstitute (mocking), EF InMemory (database), Bogus/NBuilder (test data), and follow AAA. Test files are co-located under `tests/Services/<ServiceName>/` mirroring the `Features/` structure of the source service. Tests cover both success paths and expected failure paths (e.g., duplicate detection, not-found).