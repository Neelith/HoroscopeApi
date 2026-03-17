using Hermes.Responses;
using HoroscopeApi.Application.Features.Compatibilities.GetCompatibility;
using HoroscopeApi.Application.Models;
using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Extensions;

namespace HoroscopeApi.WebApi.Endpoints.Compatibilities;

public sealed class CompatibilitiesEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("compatibilities")
            .RequireAuthorization(AuthorizationPolicies.ApiKey)
            .WithTags(Tags.Compatibilities)
            .WithDescription("Zodiac sign compatibility endpoints");

        group.MapGet("{firstSignName}/{secondSignName}", async (
                string firstSignName,
                string secondSignName,
                IQueryHandler<GetCompatibilityQuery, Response<CompatibilityData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetCompatibilityQuery query = new(firstSignName, secondSignName);
                Result<Response<CompatibilityData>> result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetCompatibility")
            .WithDescription("Get compatibility assessment between two zodiac signs.")
            .Produces<Response<CompatibilityData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}