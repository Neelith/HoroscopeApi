using Carter;
using Hermes.Handlers;
using HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;
using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HoroscopeApi.WebApi.Endpoints.Horoscopes;

public sealed class HoroscopesEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("horoscopes")
            .WithTags(Tags.Horoscopes)
            .WithDescription("Horoscope reading endpoints");

        group.MapGet("{signName}/daily", GetDailyHoroscope)
            .WithName("GetDailyHoroscope")
            .WithDescription("Get daily horoscope for a zodiac sign")
            .Produces<HoroscopeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<HoroscopeResponse>, ProblemHttpResult>> GetDailyHoroscope(
        string signName,
        DateOnly? date,
        IQueryHandler<GetDailyHoroscopeQuery, HoroscopeResponse> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetDailyHoroscopeQuery(signName, date);
        var result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value!)
            : result.ToErrorResponse();
    }
}
