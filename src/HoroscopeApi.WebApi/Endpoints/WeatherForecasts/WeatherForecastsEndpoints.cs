using Hermes.Responses;
using Microsoft.AspNetCore.Mvc;
using HoroscopeApi.Application.Features.WeatherForecasts.CreateWeatherForecasts;
using HoroscopeApi.Application.Features.WeatherForecasts.GetWeatherForecasts;
using HoroscopeApi.Domain.WeatherForecasts;
using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Extensions;

namespace HoroscopeApi.WebApi.Endpoints.WeatherForecasts;

public class WeatherForecastsEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("weatherforecasts")
            .WithTags(Tags.WeatherForecast)
            .WithDescription("Weather forecast endpoints");

        group.MapGet("", async
            ([AsParameters] GetWeatherForecastsQuery query,
            [FromServices] IQueryHandler<GetWeatherForecastsQuery, PagedResponse<WeatherForecast>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);

            IResult response = result.IsSuccess 
                ? TypedResults.Ok(result.Value)
                : result.ToErrorResponse();

            return response;
        })
        .WithDescription("Retrieves a list of weather forecasts based on the provided query parameters.")
        .Produces<PagedResponse<WeatherForecast>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("", async
            ([FromBody] CreateWeatherForecastCommand command,
            [FromServices] ICommandHandler<CreateWeatherForecastCommand, IdResponse<int>> handler) =>
        {
            var result = await handler.Handle(command, CancellationToken.None);

            IResult response = result.IsSuccess
                ? TypedResults.Created($"weatherforecasts/{result.Value?.Data.Id}", result.Value)
                : result.ToErrorResponse();

            return response;
        })
        .WithDescription("Creates a new weather forecast with the provided details.")
        .Produces<IdResponse<int>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}