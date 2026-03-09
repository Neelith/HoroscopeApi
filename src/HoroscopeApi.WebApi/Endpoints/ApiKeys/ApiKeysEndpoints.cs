using Hermes.Responses;
using HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;
using HoroscopeApi.Application.Features.ApiKeys.GetApiKeys;
using HoroscopeApi.Application.Models;
using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Extensions;

namespace HoroscopeApi.WebApi.Endpoints.ApiKeys;

public sealed class ApiKeysEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api-keys")
            .WithTags(Tags.ApiKeys)
            .WithDescription("API key management endpoints")
            // The API keys endpoints are intended for internal use and should not be exposed in public API documentation.
            // By excluding the group from description, we prevent it from appearing in generated API docs like Swagger/OpenAPI, while still allowing it to be used by authorized clients.
            .ExcludeFromDescription();

        group.MapGet("", async (
                [AsParameters] GetApiKeysQuery query,
                IQueryHandler<GetApiKeysQuery, PagedResponse<ApiKeyData>> handler,
                CancellationToken cancellationToken) =>
            {
                Result<PagedResponse<ApiKeyData>> result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetApiKeys")
            .WithDescription(
                "Get API keys with optional filters. " +
                "Use 'type' to filter by API key type (Permanent, Temporary). " +
                "Use 'rateLimitType' to filter by rate limit type (None, PerMinute, PerHour, PerDay). " +
                "Use 'ids' to filter by a comma-separated list of API key IDs.")
            .Produces<PagedResponse<ApiKeyData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("", async (
                CreateApiKeysCommands commands,
                ICommandHandler<CreateApiKeysCommands, PagedResponse<CreateApiKeyData>> handler,
                CancellationToken cancellationToken) =>
            {
                Result<PagedResponse<CreateApiKeyData>> result = await handler.Handle(commands, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Created(string.Empty, result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("CreateApiKeys")
            .WithDescription("Create one or more API keys. The server generates the key strings automatically.")
            .Produces<PagedResponse<CreateApiKeyData>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}