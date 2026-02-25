using Carter;
using Hermes.Handlers;
using Hermes.Responses;
using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.Application.Features.ZodiacSigns.GetAllZodiacSigns;
using HoroscopeApi.Application.Features.ZodiacSigns.GetZodiacSignByName;
using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HoroscopeApi.WebApi.Endpoints.ZodiacSigns;

public sealed class ZodiacSignsEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("zodiac-signs")
            .WithTags(Tags.ZodiacSigns)
            .WithDescription("Zodiac sign information endpoints");

        group.MapGet("", async (
                IQueryHandler<GetAllZodiacSignsQuery, PagedResponse<ZodiacSignInfoData>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetAllZodiacSignsQuery();
                var result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetAllZodiacSigns")
            .WithDescription("Get all zodiac signs with their information")
            .Produces<PagedResponse<ZodiacSignInfoData>>(StatusCodes.Status200OK);

        group.MapGet("{signName}", async (
                string signName,
                IQueryHandler<GetZodiacSignByNameQuery, Response<ZodiacSignInfoData>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetZodiacSignByNameQuery(signName);
                var result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetZodiacSignByName")
            .WithDescription("Get zodiac sign information by name")
            .Produces<Response<ZodiacSignInfoData>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
