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
- **Rich domain model** with factory methods (`Create`) that return `Result<T>` for validation
- **Error metadata pattern** — domain errors carry `Metadata` dictionaries with HTTP status codes (`ErrorConsts.ErrorType`), mapped to `ProblemDetails` responses via `ResultExtensions`

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

### Test Naming Conventions

| Source file                          | Test file naming                               |
|--------------------------------------|------------------------------------------------|
| Domain entity `Create` method        | `{Entity}CreateTests.cs`                       |
| Domain entity behavior method        | `{Entity}{MethodName}Tests.cs`                 |
| Application query/command handler    | `{Method}{Resource}QueryHandlerTests.cs`       |
| Application validator                | `{Method}{Resource}QueryValidatorTests.cs`     |
| Infrastructure service               | `{ServiceClass}Tests.cs`                       |

**Examples**: `HoroscopeCreateTests.cs`, `ZodiacSignInfoIsDateInRangeTests.cs`, `GetDailyHoroscopeQueryHandlerTests.cs`,
`GetDailyHoroscopeQueryValidatorTests.cs`, `RedisCacheTests.cs`

## Naming Conventions

### Features (Application layer)

Each feature lives in its own folder under `Application/Features/{Resource}/` and contains exactly three files:

| File        | Naming Pattern                                                             |
|-------------|----------------------------------------------------------------------------|
| Request DTO | `{Method}{Resource}Query` or `{Method}{Resource}Command`                   |
| Handler     | `{Method}{Resource}QueryHandler` or `{Method}{Resource}CommandHandler`     |
| Validator   | `{Method}{Resource}QueryValidator` or `{Method}{Resource}CommandValidator` |

**Queries** are used exclusively for `GET` operations. All other HTTP methods (`POST`, `PUT`, `PATCH`, `DELETE`) use
**Commands**.

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

### Endpoints (WebApi layer)

Endpoint modules implement `IEndpoints` (extends `ICarterModule`) and live under `Endpoints/{Resource}/`. Each module
groups related routes under a `MapGroup` with lowercase URL prefix, authorization policy, and OpenAPI tags.

| File naming                 | Class naming                 | Route group       |
|-----------------------------|------------------------------|-------------------|
| `HoroscopesEndpoints.cs`   | `HoroscopesEndpoints`        | `horoscopes`      |
| `ApiKeysEndpoints.cs`      | `ApiKeysEndpoints`           | `api-keys`        |
| `ZodiacSignsEndpoints.cs`  | `ZodiacSignsEndpoints`       | `zodiac-signs`    |
| `CompatibilitiesEndpoints.cs` | `CompatibilitiesEndpoints` | `compatibilities` |

### Domain Errors

Each aggregate has a static `{Entity}Errors` class with `Error` properties. Errors include metadata dictionaries mapping
to HTTP status codes via `ErrorConsts.ErrorType`.

**Examples**: `HoroscopeErrors.NotFound`, `ZodiacSignErrors.InvalidName`, `CompatibilityErrors.InvalidScore`

### Feature Examples

| Endpoint                | Request DTO              | Handler                         | Validator                         |
|-------------------------|--------------------------|---------------------------------|-----------------------------------|
| `GET /horoscopes/daily` | `GetDailyHoroscopeQuery` | `GetDailyHoroscopeQueryHandler` | `GetDailyHoroscopeQueryValidator` |
| `POST /api-keys`        | `CreateApiKeysCommand`   | `CreateApiKeysCommandHandler`   | `CreateApiKeysCommandValidator`   |
| `GET /api-keys`         | `GetApiKeysQuery`        | `GetApiKeysQueryHandler`        | `GetApiKeysQueryValidator`        |

## Core Business Logic

The API generates AI-powered horoscope predictions for all 12 zodiac signs across multiple time periods (daily, weekly,
monthly, yearly). When a horoscope is requested, the system first checks the Redis cache, then the database; if neither
has a result, it generates a new prediction via HuggingFace (Llama 3.1 8B Instruct), persists it to PostgreSQL, and
caches it in Redis. The same generate-or-retrieve pattern applies to zodiac sign compatibility assessments. API access is
controlled through API keys (created and validated internally) with configurable rate limits and scopes, while user
authentication flows through Keycloak JWTs.

## Entities

### Horoscope

**Purpose**: An AI-generated horoscope prediction for a specific zodiac sign, period, and date.

**Key Properties**: `ZodiacSignId`, `Period` (Daily/Weekly/Monthly/Yearly), `Date`, `GeneralPrediction`,
`LovePrediction`, `CareerPrediction`, `HealthPrediction`, `LuckyNumbers`, `LuckyColors`, `MoodScore`, `Keywords`

**Relationships**: Belongs to one `ZodiacSignInfo` (via `ZodiacSignId`).

### ZodiacSignInfo

**Purpose**: Reference data for a zodiac sign — its date range, element, quality, polarity, ruling planet, and
description.

**Key Properties**: `Sign` (enum), `Name`, `Symbol`, `StartMonth/StartDay`, `EndMonth/EndDay`, `Element`, `Quality`,
`Polarity`, `RulingPlanet`, `Description`

**Relationships**: Has many `Horoscope` records. Referenced by `Compatibility` (as both first and second sign).

### Compatibility

**Purpose**: An AI-generated compatibility assessment between two zodiac signs, with a score and description.

**Key Properties**: `FirstZodiacSignId`, `SecondZodiacSignId`, `Score` (0–100), `Description`

**Relationships**: References two `ZodiacSignInfo` entities (`FirstZodiacSignInfo`, `SecondZodiacSignInfo`).

### PromptTemplate

**Purpose**: Stores the system prompt, few-shot examples, and user prompt template used to generate AI predictions.

**Key Properties**: `Type` (Daily/Weekly/Monthly/Yearly/Compatibility), `SystemPrompt`, `FewShotExamples`,
`UserPromptTemplate`

**Relationships**: Standalone — referenced at generation time by `PromptType`.

### ApiKey

**Purpose**: An API key issued to consumers for accessing the horoscope endpoints. Stores a hashed version of the key
with salt for secure validation.

**Key Properties**: `OwnerId` (Guid), `Prefix`, `Hash`, `Salt`, `Algorithm`, `Type` (Permanent/Temporary),
`RateLimitType` (None/PerMinute/PerHour/PerDay), `RateLimitCount`, `RateLimit`, `ExpiresAtUtc`

**Relationships**: Has many `ApiKeyScope` records.

### ApiKeyScope

**Purpose**: Defines a named permission scope attached to an API key, controlling what the key is authorized to access.

**Key Properties**: `ApiKeyId`, `Name`

**Relationships**: Belongs to one `ApiKey`.
