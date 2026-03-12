using Hermes.Responses;
using HoroscopeApi.Application.Features.ZodiacSigns.GetAllZodiacSigns;
using HoroscopeApi.Application.Features.ZodiacSigns.GetZodiacSignByName;
using HoroscopeApi.Application.Models;
using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Extensions;

namespace HoroscopeApi.WebApi.Endpoints.ZodiacSigns;

public sealed class ZodiacSignsEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("zodiac-signs")
            .RequireAuthorization(AuthorizationPolicies.ApiKey)
            .WithTags(Tags.ZodiacSigns)
            .WithDescription("Zodiac sign information endpoints");

        group.MapGet("", async (
                IQueryHandler<GetAllZodiacSignsQuery, PagedResponse<ZodiacSignInfoData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetAllZodiacSignsQuery query = new();
                Result<PagedResponse<ZodiacSignInfoData>> result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetAllZodiacSigns")
            .WithDescription("Get all zodiac signs with their information")
            .Produces<PagedResponse<ZodiacSignInfoData>>();

        group.MapGet("{signName}", async (
                string signName,
                IQueryHandler<GetZodiacSignByNameQuery, Response<ZodiacSignInfoData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetZodiacSignByNameQuery query = new(signName);
                Result<Response<ZodiacSignInfoData>> result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetZodiacSignByName")
            .WithDescription("Get zodiac sign information by name")
            .Produces<Response<ZodiacSignInfoData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}