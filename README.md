# Vladify.AdminApi

Admin & moderation service for the **Vladify** music platform. It manages the
lifecycle of song *moderation tasks*: songs submitted for review are turned into
tasks, moderators pull them off a queue one at a time, and then approve or reject
them.

The service exposes two surfaces:

- A **REST API** used by moderators (assign / approve / reject tasks).
- A **gRPC API** used by other internal services to create moderation tasks
  (e.g. when a new song is uploaded).

---

## Tech stack

| Area            | Technology                                                                 |
| --------------- | -------------------------------------------------------------------------- |
| Runtime         | .NET 10 / ASP.NET Core                                                      |
| Architecture    | Clean Architecture + CQRS ([MediatR](https://github.com/jbogard/MediatR))  |
| Persistence     | PostgreSQL via EF Core 10 (`Npgsql`) + [Dapper](https://github.com/DapperLib/Dapper) for hot-path queries |
| Resilience      | [Polly](https://github.com/App-vNext/Polly) (retry with exponential backoff on transient DB errors) |
| Auth            | JWT Bearer via [Auth0](https://auth0.com/) (OAuth2 Authorization Code + PKCE) |
| RPC             | gRPC (`Grpc.AspNetCore`)                                                    |
| API docs        | OpenAPI + [Scalar](https://github.com/scalar/scalar) (Development only)     |
| Mapping         | AutoMapper                                                                  |
| Config          | `.env` files via [DotNetEnv](https://github.com/tonerdo/dotnet-env) / user secrets |
| Tests           | xUnit, Moq, AutoFixture, FluentAssertions                                   |
| CI              | GitHub Actions + SonarCloud (build, test, coverage)                         |

---

## Solution structure

The solution follows Clean Architecture. Dependencies point inward: outer layers
depend on inner layers, never the reverse.

```
Vladify.AdminApi.slnx
├── Vladify.Domain           # Enterprise core — no external dependencies
│   ├── Entities             #   ModerationTask
│   ├── Enums                #   ModerationStatus (Pending / Approved / Rejected)
│   └── Constants
│
├── Vladify.Application      # Use cases (depends on Domain)
│   ├── Commands             #   CQRS handlers: CreateTask, AssignTask, ApproveTask, RejectTask
│   ├── Interfaces           #   IModerationTaskRepository
│   ├── MapperProfiles       #   AutoMapper profiles
│   ├── Exceptions           #   Domain-specific exceptions
│   ├── Options              #   Auth0Options
│   └── Constants            #   AppRoles, ErrorMessages
│
├── Vladify.Infrastructure   # I/O concerns (depends on Application + Domain)
│   ├── ApplicationDbContext #   EF Core DbContext
│   ├── Configurations       #   EF entity configurations
│   ├── Repositories         #   ModerationTaskRepository (EF Core + Dapper)
│   ├── Migrations           #   EF Core migrations
│   └── Extensions           #   DI registration, DB migration helper
│
├── Vladify.AdminApi         # Presentation / host (depends on Application + Infrastructure)
│   ├── Controllers          #   REST: TasksController
│   ├── Grpc                 #   gRPC service, proto contract, exception interceptor
│   ├── Extensions           #   Auth, OpenAPI/Scalar, gRPC & endpoint wiring
│   ├── Config               #   EnvLoader
│   └── Program.cs           #   Composition root
│
└── Vladify.Tests            # Unit tests for command handlers
```

---

## API reference

### REST — `/api/tasks`

All endpoints require a valid JWT with the **`Admin`** or **`Moderator`** role
(policy `AdminOrModerator`). The role is read from the
`https://Vladify.com/roles` claim, and the moderator identity from the `sub`
(NameIdentifier) claim.

| Method | Route                      | Description                                                            |
| ------ | -------------------------- | ---------------------------------------------------------------------- |
| `POST` | `/api/tasks/assign`        | Claims the next pending task from the FIFO queue for the current moderator. Returns the task id, or `null` if the queue is empty. |
| `PUT`  | `/api/tasks/{id}/approve`  | Approves a task assigned to the current moderator.                     |
| `PUT`  | `/api/tasks/{id}/reject`   | Rejects a task with a reason (`rejactionReason` query parameter — spelling matches the source). |

### gRPC — `ModerationGrpc`

Defined in [`moderation.proto`](Vladify.AdminApi/Grpc/ModerationTask/moderation.proto).
Intended for service-to-service calls.

| RPC          | Request                | Response                | Description                                  |
| ------------ | ---------------------- | ----------------------- | -------------------------------------------- |
| `CreateTask` | `CreateTaskRequest { song_id }` | `CreateTaskResponse { task_id }` | Creates a new `Pending` moderation task for a song. |

---

## Business rules

- **One active task per moderator.** A moderator cannot claim a new task while
  they already have one assigned.
- **FIFO queue.** Assigning claims the oldest `Pending`, unassigned task.
- **Concurrency-safe claiming.** Task assignment uses PostgreSQL
  `FOR UPDATE SKIP LOCKED`, so concurrent moderators never grab the same task.
- **Ownership enforcement.** A task can only be approved/rejected by the
  moderator it is assigned to, and only after it has been claimed.
- **One task per song.** `SongId` is unique.
- **Rejection message** is capped at 250 characters.

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A PostgreSQL instance
- An Auth0 tenant (for issuing/validating JWTs)

### 1. Configure environment

Copy the example environment file and fill in your values:

```bash
cp .env.example .env
```

Configuration is loaded from `.env` (which traverses up the directory tree) or
from .NET user secrets. Key settings:

| Variable                          | Description                                             |
| --------------------------------- | ------------------------------------------------------- |
| `ConnectionStrings__AdminApiDb`   | PostgreSQL connection string used by the app.           |
| `Domain`                          | Auth0 domain (without `https://`).                      |
| `ClientId`                        | Auth0 application client id (used by the Scalar docs UI). |
| `Audience`                        | Auth0 API audience, e.g. `https://Vladify/musicAPI`.    |
| `DbPassword` / `DbName` / `DbUser`| Postgres container settings for a Docker Compose setup. |

> The `.env` file is git-ignored by default — do not commit real secrets.

### 2. Run the API

```bash
dotnet run --project Vladify.AdminApi
```

Database migrations are applied automatically on startup, so no manual
`dotnet ef database update` step is required.

Local URLs (see `Properties/launchSettings.json`):

- HTTP: `http://localhost:5023`
- HTTPS: `https://localhost:7211`

In the **Development** environment, interactive API documentation is available
via Scalar at `/scalar` (with OpenAPI at `/openapi/v1.json`).

### 3. Run the tests

```bash
dotnet test
```

---

## Running with Docker

A Linux [`Dockerfile`](Vladify.AdminApi/Dockerfile) is provided (exposes port
`8080`):

```bash
docker build -t vladify-adminapi -f Vladify.AdminApi/Dockerfile .
docker run -p 8080:8080 --env-file .env vladify-adminapi
```

---

## Database migrations

Migrations live in `Vladify.Infrastructure/Migrations` and run automatically at
startup. To create a new migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project Vladify.Infrastructure \
  --startup-project Vladify.AdminApi
```

---

## Continuous integration

[`.github/workflows/ci.yml`](.github/workflows/ci.yml) runs on pushes and pull
requests to `main`: it restores, builds in `Release`, runs the test suite with
coverage, and reports to SonarCloud. It expects a `SONAR_TOKEN` repository
secret.

---

## License

Distributed under the MIT License. See [`LICENSE`](LICENSE) for details.
