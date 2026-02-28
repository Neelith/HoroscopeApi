using Hermes.Responses;
using HoroscopeApi.Application.Features.Horoscopes.GetHoroscope;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Extensions;

namespace HoroscopeApi.WebApi.Endpoints.Horoscopes;

public sealed class HoroscopesEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("horoscopes")
            .WithTags(Tags.Horoscopes)
            .WithDescription("Horoscope reading endpoints");

        group.MapGet("{signName}", async (
                string signName,
                HoroscopePeriod? period,
                DateOnly? date,
                IQueryHandler<GetHoroscopeQuery, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetHoroscopeQuery query = new(signName, period, date);
                Result<Response<HoroscopeData>> result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetHoroscope")
            .WithDescription(
                "Get horoscope for a zodiac sign. " +
                "Use 'period' query parameter to specify the horoscope period (daily, weekly, monthly, yearly). Defaults to daily if not specified. " +
                "Use 'date' query parameter to get horoscope for a specific date (ISO format: yyyy-MM-dd). " +
                "If 'date' is provided, it takes precedence and the horoscope will be daily regardless of the 'period' parameter.")
            .Produces<Response<HoroscopeData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}