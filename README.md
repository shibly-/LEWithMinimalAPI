# Clean Architecture Web API (.NET 10, Minimal API + CQRS)

A reference ASP.NET Core Web API built from the project's master spec, following
**Clean Architecture** with **CQRS/MediatR**, **FluentValidation**, **AutoMapper**,
**EF Core 10 (SQL Server)**, a dedicated **migrations project**, and **Scalar** API docs.

## Solution layout

```
LEWithMinimalAPI.slnx
├─ src/
│  ├─ Domain/          # Pure domain model. No EF/infra dependencies.
│  │                   #   - Entities/Organization.cs (invariants + behavior)
│  ├─ Application/     # Use cases (CQRS). Depends only on Domain.
│  │                   #   - Organizations/Commands, Queries, Dtos, Mappings
│  │                   #   - Common/Behaviors/ValidationBehavior.cs (MediatR pipeline)
│  │                   #   - Common/Interfaces/IApplicationDbContext.cs
│  │                   #   - Common/Exceptions (NotFound, Validation)
│  │                   #   - DependencyInjection.cs (AddApplication)
│  ├─ Infrastructure/  # EF Core. Implements IApplicationDbContext (no repositories).
│  │                   #   - Persistence/ApplicationDbContext.cs + Configurations
│  │                   #   - DependencyInjection.cs (AddInfrastructure)
│  ├─ Migrations/      # Dedicated EF migrations + seeding project.
│  │                   #   - Persistence/Migrations/* (InitialCreate)
│  │                   #   - DesignTimeDbContextFactory.cs
│  │                   #   - DbInitializer.cs (migrate + seed default records)
│  └─ Api/             # Minimal API host. Composition root.
│                      #   - Program.cs (DI wiring, migrate/seed, Scalar)
│                      #   - Endpoints/OrganizationEndpoints.cs
│                      #   - Infrastructure/GlobalExceptionHandler.cs (ProblemDetails)
└─ tests/
   ├─ Domain.UnitTests/        # Entity invariants & behavior (xUnit)
   └─ Application.UnitTests/    # Command/query handlers & validators (xUnit + EF InMemory)
```

Dependency direction: `Api → Application → Domain`, `Infrastructure → Application → Domain`,
`Migrations → Infrastructure`. The Domain layer depends on nothing.

## Tech choices

| Concern         | Choice                                       |
| --------------- | -------------------------------------------- |
| Framework       | .NET 10 (`net10.0`)                          |
| Database        | SQL Server (`localhost\SQLEXPRESS`)          |
| ORM             | EF Core 10                                   |
| Messaging/CQRS  | MediatR 14                                   |
| Validation      | FluentValidation 12 (via MediatR pipeline)   |
| Mapping         | AutoMapper 16                                |
| API docs        | Scalar + Microsoft OpenAPI                   |
| Tests           | xUnit, Moq, EF Core InMemory                 |

> **Licensing note:** MediatR (v13+) and AutoMapper (v15+) moved to a dual
> commercial/OSS license. They remain free for development/testing and for
> organizations under $5M USD annual revenue, and run without any runtime block
> (a missing license key only produces an informational log message). To register
> a free key, set it in `AddMediatR`/`AddAutoMapper` or via the
> `LUCKYPENNY_LICENSE_KEY` environment variable.

## Endpoints

| Method | Route                      | Body                              | Success | Validation |
| ------ | -------------------------- | --------------------------------- | ------- | ---------- |
| GET    | `/api/organizations/{id}`  | –                                 | 200     | `id > 0`   |
| POST   | `/api/organizations`       | `{ "name", "description?" }`      | 201     | name required, 3–100 chars; description ≤ 250 |

Errors are returned as RFC 7807 `ProblemDetails`:
- **400** – validation failures (with a per-field `errors` dictionary) or malformed JSON
- **404** – organization not found
- **500** – unexpected errors

## Prerequisites

- .NET 10 SDK
- A reachable SQL Server instance. The default connection string targets
  `localhost\SQLEXPRESS` with SQL auth (`sqluser` / `password`) and creates the
  `LEMinimalAPIDB` database automatically on first run. Adjust it in
  `src/Api/appsettings.json` (`ConnectionStrings:DefaultConnection`) if needed.

## Running

```bash
# from the solution root
dotnet run --project src/Api

# then open the interactive docs
#   http://localhost:5071/scalar/v1   (http profile)
```

On startup the API applies pending EF Core migrations (creating the database if
absent) and seeds two default organizations ("Acme Corporation", "Globex Corporation").

### Example requests

```bash
# Get the first seeded organization
curl http://localhost:5071/api/organizations/1

# Create an organization
curl -X POST http://localhost:5071/api/organizations \
     -H "Content-Type: application/json" \
     -d '{"name":"Initech","description":"A software company"}'
```

## Tests

```bash
dotnet test
```

## EF Core migrations

Migrations live in the dedicated `Migrations` project and use its
`DesignTimeDbContextFactory`, so no running host is required:

```bash
# add a new migration
dotnet ef migrations add <Name> \
  --project src/Migrations --startup-project src/Migrations \
  --output-dir Persistence/Migrations

# apply migrations manually (also done automatically on API startup)
dotnet ef database update \
  --project src/Migrations --startup-project src/Migrations
```

## Notes

- The connection string is configured in `src/Api/appsettings.json`
  (`ConnectionStrings:DefaultConnection`).
- No repository pattern: handlers use `IApplicationDbContext` directly, per the spec.
- All NuGet packages are on their latest versions (EF Core 10, MediatR 14,
  AutoMapper 16, FluentValidation 12, Scalar 2.x) — see the licensing note above.
