using Hermes.Handlers;
using Hermes.Responses;
using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Application.Features.ZodiacSigns.GetZodiacSignByName;

internal sealed class GetZodiacSignByNameQueryHandler(IZodiacSignRepository repository)
    : IQueryHandler<GetZodiacSignByNameQuery, Response<ZodiacSignInfoData>>
{
    public async Task<Result<Response<ZodiacSignInfoData>>> Handle(
        GetZodiacSignByNameQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ZodiacSign>(query.SignName, true, out var zodiacSign))
        {
            return Result.Ko<Response<ZodiacSignInfoData>>(ZodiacSignErrors.InvalidName);
        }

        var repositoryQuery = new GetZodiacSignBySignRepositoryQuery(zodiacSign);
        var signInfo = await repository.GetBySignAsync(repositoryQuery, cancellationToken);

        if (signInfo is null)
        {
            return Result.Ko<Response<ZodiacSignInfoData>>(ZodiacSignErrors.NotFound(zodiacSign));
        }

        var data = new ZodiacSignInfoData
        {
            Name = signInfo.Name,
            Symbol = signInfo.Symbol,
            Element = signInfo.Element.ToString(),
            Quality = signInfo.Quality.ToString(),
            Polarity = signInfo.Polarity.ToString(),
            RulingPlanet = signInfo.RulingPlanet,
            DateRange = GetDateRangeString(signInfo.StartMonth, signInfo.StartDay, signInfo.EndMonth, signInfo.EndDay),
            Description = signInfo.Description
        };

        var response = Response<ZodiacSignInfoData>.Create(data);
        return Result.Ok(response);
    }

    private static string GetDateRangeString(int startMonth, int startDay, int endMonth, int endDay)
    {
        var startMonthName = new DateOnly(2000, startMonth, 1).ToString("MMMM");
        var endMonthName = new DateOnly(2000, endMonth, 1).ToString("MMMM");
        return $"{startMonthName} {startDay} - {endMonthName} {endDay}";
    }
}
