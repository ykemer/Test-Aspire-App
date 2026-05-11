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

## Testing

```bash
# Run all tests
dotnet test "Aspire App.sln"

# Run tests for a specific service
dotnet test tests/Services/Courses/Test.Courses/Test.Courses.csproj
dotnet test tests/Services/Students/Test.Students/Test.Students.csproj
dotnet test tests/Services/Test.Enrollments/Test.Enrollments.csproj

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