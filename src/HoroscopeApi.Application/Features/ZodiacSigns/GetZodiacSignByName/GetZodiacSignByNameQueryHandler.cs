using HoroscopeApi.Application.Models;
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
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<ZodiacSignInfoData>>(ZodiacSignErrors.InvalidName);
        }

        GetZodiacSignBySignRepositoryQuery repositoryQuery = new(zodiacSign);
        ZodiacSignInfo? signInfo = await repository.GetBySignAsync(repositoryQuery, cancellationToken);

        if (signInfo is null)
        {
            return Result.Ko<Response<ZodiacSignInfoData>>(ZodiacSignErrors.NotFound(zodiacSign));
        }

        ZodiacSignInfoData data = new()
        {
            Name = signInfo.Name,
            Symbol = signInfo.Symbol,
            Element = signInfo.Element.ToString(),
            Quality = signInfo.Quality.ToString(),
            Polarity = signInfo.Polarity.ToString(),
            RulingPlanet = signInfo.RulingPlanet,
            DateRange = GetDateRangeString(signInfo.StartMonth, signInfo.StartDay, signInfo.EndMonth,
                signInfo.EndDay),
            Description = signInfo.Description
        };

        Response<ZodiacSignInfoData> response = Response<ZodiacSignInfoData>.Create(data);
        return Result.Ok(response);
    }

    private static string GetDateRangeString(int startMonth, int startDay, int endMonth, int endDay)
    {
        string startMonthName = new DateOnly(2000, startMonth, 1).ToString("MMMM");
        string endMonthName = new DateOnly(2000, endMonth, 1).ToString("MMMM");
        return $"{startMonthName} {startDay} - {endMonthName} {endDay}";
    }
}