# AGENTS.md

## Project Overview

HoroscopeApi is a .NET 10 Web API built with **Clean Architecture**. It provides horoscope predictions powered by
HuggingFace (Llama 3.1 8B Instruct), with PostgreSQL for persistence, Redis for caching, and Keycloak for
authentication.

## Architecture

The solution follows Clean Architecture with strict dependency direction:

```
Domain <-- Application <-- Infrastructure <-- WebApi
```

| Layer              | Project                       | Responsibility                                                                                                                        |
|--------------------|-------------------------------|---------------------------------------------------------------------------------------------------------------------------------------|
| **Domain**         | `HoroscopeApi.Domain`         | Entities, enums, value objects, repository interfaces, domain events. Zero infrastructure dependencies.                               |
| **Application**    | `HoroscopeApi.Application`    | CQRS handlers (via Hermes), FluentValidation validators, service interfaces. References Domain only.                                  |
| **Infrastructure** | `HoroscopeApi.Infrastructure` | EF Core DbContext, repository implementations, Redis cache, HuggingFace client, all service implementations. References Application.  |
| **WebApi**         | `HoroscopeApi.WebApi`         | Carter minimal API endpoints, authentication/authorization, middleware, DI composition root. References Application + Infrastructure. |
| **Tests**          | `HoroscopeApi.Tests`          | Unit tests with xUnit + Moq. References Application + Infrastructure.                                                                 |

### Key Patterns

- **CQRS** via the `Neelith.Hermes` library (`IQueryHandler<TQuery, TResult>`, `ICommandHandler<TCommand, TResult>`)
- **Decorator pattern** for cross-cutting concerns (Validation, Logging) applied via Scrutor
- **Repository pattern** with interfaces in Domain, implementations in Infrastructure
- **Unit of Work** pattern (`ApplicationDbContext` implements `IUnitOfWork`)
- **Minimal API** with Carter endpoint modules (no controllers)
- **Feature-folder** organization under `Application/Features/`

## Build

Always build the solution using the `.slnx` file:

```bash
dotnet build src/src.slnx
```

## Migrations

Every EF Core migration command **must** use the `--project`, `--startup-project`, and `-o` flags.

Add a migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/HoroscopeApi.Infrastructure \
  --startup-project src/HoroscopeApi.WebApi \
  -o Persistence/Migrations
```

Update the database:

```bash
dotnet ef database update \
  --project src/HoroscopeApi.Infrastructure \
  --startup-project src/HoroscopeApi.WebApi
```

Remove the last migration:

```bash
dotnet ef migrations remove \
  --project src/HoroscopeApi.Infrastructure \
  --startup-project src/HoroscopeApi.WebApi
```

Do **not** run `dotnet ef` commands without these flags. The DbContext lives in Infrastructure but the connection string
is resolved from WebApi.

## Testing

Run all tests targeting the `.slnx` file:

```bash
dotnet test src/src.slnx
```

### Rules

- Every new feature or change **must** include corresponding unit tests.
- Test project structure mirrors the source layers: `Tests/Domain/`, `Tests/Application/Features/`,
  `Tests/Infrastructure/`.
- Use **xUnit** for test framework, **Moq** for mocking, and **FluentValidation** for validator tests.

## Naming Conventions

### Features (Application layer)

Each feature lives in its own folder under `Application/Features/{Resource}/` and contains exactly three files:

| File        | Naming Pattern                                                             |
|-------------|----------------------------------------------------------------------------|
| Request DTO | `{Method}{Resource}Query` or `{Method}{Resource}Command`                   |
| Handler     | `{Method}{Resource}QueryHandler` or `{Method}{Resource}CommandHandler`     |
| Validator   | `{Method}{Resource}QueryValidator` or `{Method}{Resource}CommandValidator` |

**Queries** are used exclusively for `GET` operations. All other HTTP methods (`POST`, `PUT`, `PATCH`, `DELETE`) use *
*Commands**.

Parameterless queries (records with no properties) are exempt from the three-file requirement — they do not need a
validator.

Validators **must** always contain meaningful validation rules. Empty validators (with no rules defined) are not
permitted.

### Repository DTOs (Domain layer)

Each repository method has its own dedicated DTO, suffixed with `RepositoryQuery` (for reads) or `RepositoryCommand` (
for writes):

- `GetZodiacSignBySignRepositoryQuery`
- `GetHoroscopesByDateRangeRepositoryQuery`
- `UpsertApiKeysRepositoryCommand`

### Examples

| Endpoint                | Request DTO              | Handler                         | Validator                         |
|-------------------------|--------------------------|---------------------------------|-----------------------------------|
| `GET /horoscopes/daily` | `GetDailyHoroscopeQuery` | `GetDailyHoroscopeQueryHandler` | `GetDailyHoroscopeQueryValidator` |
| `POST /api-keys`        | `CreateApiKeysCommand`   | `CreateApiKeysCommandHandler`   | `CreateApiKeysCommandValidator`   |
| `GET /api-keys`         | `GetApiKeysQuery`        | `GetApiKeysQueryHandler`        | `GetApiKeysQueryValidator`        |
