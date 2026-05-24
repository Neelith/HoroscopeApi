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
- **Rich domain model** with factory methods (`Create`) that return `Result<T>` for validation (most entities). Exception: `ApiKey` and `ApiKeyScope` use a simpler pattern with `required` properties and public setters.
- **Domain events** — base `Entity` class supports `Raise()` and `ClearDomainEvents()`, with `IDomainEvent` and `IDomainEventHandler` interfaces
- **Error metadata pattern** — domain errors carry `Metadata` dictionaries with HTTP status codes (`ErrorConsts.ErrorType`), mapped to `ProblemDetails` responses via `ResultExtensions`

## Naming Conventions

### Features (Application layer)

Each feature lives in a folder under `Application/Features/{Resource}/` and contains up to three files:

| File        | Naming Pattern                                                          |
|-------------|-------------------------------------------------------------------------|
| Request DTO | `{Method}{Resource}Query` (GET) or `{Method}{Resource}Command` (others) |
| Handler     | `{Method}{Resource}QueryHandler` or `{Method}{Resource}CommandHandler`  |
| Validator   | `{Method}{Resource}QueryValidator` or `{Method}{Resource}CommandValidator` |

**Queries** are used exclusively for `GET` operations. All other HTTP methods use **Commands**.

Parameterless queries (records with no properties) are exempt from the three-file requirement — they have no validator.

Validators **must** always contain meaningful validation rules. Empty validators (with no rules defined) are not permitted.

### Repository DTOs (Domain layer)

Each repository method has its own dedicated DTO, suffixed with `RepositoryQuery` (for reads) or `RepositoryCommand` (for writes):

- `GetZodiacSignBySignRepositoryQuery`
- `GetHoroscopesByDateRangeRepositoryQuery`
- `UpsertApiKeysRepositoryCommand`

### Domain Errors

Each aggregate has a static `{Entity}Errors` class with `Error` properties carrying metadata dictionaries.

**Examples**: `HoroscopeErrors.NotFound`, `ZodiacSignErrors.InvalidName`, `CompatibilityErrors.InvalidScore`

## Testing

Run tests via the solution file:

```bash
dotnet test src/src.slnx
```

### Rules

- Every feature/change **must** include corresponding unit tests.
- Test project mirrors source layers: `Tests/Domain/`, `Tests/Application/Features/`, `Tests/Infrastructure/`.
- Use **xUnit** for framework, **Moq** for mocking, **FluentValidation** for validator tests.

### Test Naming Conventions

| Source file                          | Test file naming                               |
|--------------------------------------|------------------------------------------------|
| Domain entity `Create` method        | `{Entity}CreateTests.cs`                       |
| Domain entity behavior method        | `{Entity}{MethodName}Tests.cs`                 |
| Application query/command handler    | `{Method}{Resource}QueryHandlerTests.cs`       |
| Application validator                | `{Method}{Resource}QueryValidatorTests.cs`     |
| Infrastructure service               | `{ServiceClass}Tests.cs`                       |

**Examples**: `HoroscopeCreateTests.cs`, `ZodiacSignInfoIsDateInRangeTests.cs`,
`GetDailyHoroscopeQueryHandlerTests.cs`, `GetDailyHoroscopeQueryValidatorTests.cs`, `RedisCacheTests.cs`

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

**Key Properties**: `Sign` (enum), `Name`, `Symbol`, `StartMonth`/`StartDay`, `EndMonth`/`EndDay`, `Element`, `Quality`,
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

**Key Properties**: `OwnerId` (Guid), `Name`, `Prefix`, `Hash`, `Salt`, `Algorithm`, `Type` (Permanent/Temporary),
`RateLimitType` (None/PerMinute/PerHour/PerDay), `RateLimit`, `ExpiresAtUtc`

**Relationships**: Has many `ApiKeyScope` records.

### ApiKeyScope

**Purpose**: Defines a named permission scope attached to an API key, controlling what the key is authorized to access.

**Key Properties**: `ApiKeyId`, `Name`

**Relationships**: Belongs to one `ApiKey`.
