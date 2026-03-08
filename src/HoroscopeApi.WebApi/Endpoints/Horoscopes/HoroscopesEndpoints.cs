using Hermes.Responses;
using HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;
using HoroscopeApi.Application.Features.Horoscopes.GetDateHoroscope;
using HoroscopeApi.Application.Features.Horoscopes.GetMonthlyHoroscope;
using HoroscopeApi.Application.Features.Horoscopes.GetWeeklyHoroscope;
using HoroscopeApi.Application.Features.Horoscopes.GetYearlyHoroscope;
using HoroscopeApi.Application.Models;
using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Extensions;

namespace HoroscopeApi.WebApi.Endpoints.Horoscopes;

public sealed class HoroscopesEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("horoscopes")
            .RequireAuthorization(AuthorizationPolicies.ApiKey)
            .WithTags(Tags.Horoscopes)
            .WithDescription("Horoscope reading endpoints");

        group.MapGet("{signName}/daily", async (
                string signName,
                IQueryHandler<GetDailyHoroscopeRequest, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetDailyHoroscopeRequest request = new(signName);
                Result<Response<HoroscopeData>> result = await handler.Handle(request, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetDailyHoroscope")
            .WithDescription("Get today's daily horoscope for a zodiac sign.")
            .Produces<Response<HoroscopeData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("{signName}/weekly", async (
                string signName,
                IQueryHandler<GetWeeklyHoroscopeRequest, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetWeeklyHoroscopeRequest request = new(signName);
                Result<Response<HoroscopeData>> result = await handler.Handle(request, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetWeeklyHoroscope")
            .WithDescription("Get this week's horoscope for a zodiac sign.")
            .Produces<Response<HoroscopeData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("{signName}/monthly", async (
                string signName,
                IQueryHandler<GetMonthlyHoroscopeRequest, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetMonthlyHoroscopeRequest request = new(signName);
                Result<Response<HoroscopeData>> result = await handler.Handle(request, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetMonthlyHoroscope")
            .WithDescription("Get this month's horoscope for a zodiac sign.")
            .Produces<Response<HoroscopeData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("{signName}/yearly", async (
                string signName,
                int? year,
                IQueryHandler<GetYearlyHoroscopeRequest, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetYearlyHoroscopeRequest request = new(signName, year);
                Result<Response<HoroscopeData>> result = await handler.Handle(request, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetYearlyHoroscope")
            .WithDescription(
                "Get this year's horoscope for a zodiac sign. " +
                "Optionally specify a 'year' query parameter (defaults to current year).")
            .Produces<Response<HoroscopeData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("{signName}/date/{date}", async (
                string signName,
                DateOnly date,
                IQueryHandler<GetDateHoroscopeRequest, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetDateHoroscopeRequest request = new(signName, date);
                Result<Response<HoroscopeData>> result = await handler.Handle(request, cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value!)
                    : result.ToErrorResponse();
            })
            .WithName("GetDateHoroscope")
            .WithDescription("Get horoscope for a zodiac sign on a specific date (ISO format: yyyy-MM-dd).")
            .Produces<Response<HoroscopeData>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
