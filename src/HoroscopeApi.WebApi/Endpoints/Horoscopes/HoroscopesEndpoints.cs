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
                IQueryHandler<GetDailyHoroscopeQuery, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetDailyHoroscopeQuery query = new(signName);
                Result<Response<HoroscopeData>> result = await handler.Handle(query, cancellationToken);

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
                IQueryHandler<GetWeeklyHoroscopeQuery, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetWeeklyHoroscopeQuery query = new(signName);
                Result<Response<HoroscopeData>> result = await handler.Handle(query, cancellationToken);

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
                IQueryHandler<GetMonthlyHoroscopeQuery, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetMonthlyHoroscopeQuery query = new(signName);
                Result<Response<HoroscopeData>> result = await handler.Handle(query, cancellationToken);

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
                IQueryHandler<GetYearlyHoroscopeQuery, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetYearlyHoroscopeQuery query = new(signName, year);
                Result<Response<HoroscopeData>> result = await handler.Handle(query, cancellationToken);

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
                IQueryHandler<GetDateHoroscopeQuery, Response<HoroscopeData>> handler,
                CancellationToken cancellationToken) =>
            {
                GetDateHoroscopeQuery query = new(signName, date);
                Result<Response<HoroscopeData>> result = await handler.Handle(query, cancellationToken);

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