using Hermes.Handlers;
using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;
using System.Text.Json;

namespace HoroscopeApi.Application.Features.ZodiacSigns.GetAllZodiacSigns;

internal sealed class GetAllZodiacSignsQueryHandler(IZodiacSignRepository repository)
    : IQueryHandler<GetAllZodiacSignsQuery, ZodiacSignListResponse>
{
    public async Task<Result<ZodiacSignListResponse>> Handle(
        GetAllZodiacSignsQuery query,
        CancellationToken cancellationToken)
    {
        var zodiacSigns = await repository.GetAllAsync(cancellationToken);

        var signsList = zodiacSigns.Select(z => new ZodiacSignInfoResponse
        {
            Name = z.Name,
            Symbol = z.Symbol,
            Element = z.Element.ToString(),
            Quality = z.Quality.ToString(),
            Polarity = z.Polarity.ToString(),
            RulingPlanet = z.RulingPlanet,
            DateRange = GetDateRangeString(z.StartMonth, z.StartDay, z.EndMonth, z.EndDay),
            Description = z.Description
        }).ToList();

        var response = new ZodiacSignListResponse(signsList);
        return Result.Ok(response);
    }

    private static string GetDateRangeString(int startMonth, int startDay, int endMonth, int endDay)
    {
        var startMonthName = new DateOnly(2000, startMonth, 1).ToString("MMMM");
        var endMonthName = new DateOnly(2000, endMonth, 1).ToString("MMMM");
        return $"{startMonthName} {startDay} - {endMonthName} {endDay}";
    }
}
