using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Application.Features.ZodiacSigns.GetAllZodiacSigns;

internal sealed class GetAllZodiacSignsQueryHandler(IZodiacSignRepository repository)
    : IQueryHandler<GetAllZodiacSignsQuery, PagedResponse<ZodiacSignInfoData>>
{
    public async Task<Result<PagedResponse<ZodiacSignInfoData>>> Handle(
        GetAllZodiacSignsQuery query,
        CancellationToken cancellationToken)
    {
        List<ZodiacSignInfo> zodiacSigns = await repository.GetAllAsync(cancellationToken);

        List<ZodiacSignInfoData> dataList = zodiacSigns.Select(z => new ZodiacSignInfoData
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

        PagedResponse<ZodiacSignInfoData> response = PagedResponse<ZodiacSignInfoData>.Create(dataList, dataList.Count);
        return Result.Ok(response);
    }

    private static string GetDateRangeString(int startMonth, int startDay, int endMonth, int endDay)
    {
        string startMonthName = new DateOnly(2000, startMonth, 1).ToString("MMMM");
        string endMonthName = new DateOnly(2000, endMonth, 1).ToString("MMMM");
        return $"{startMonthName} {startDay} - {endMonthName} {endDay}";
    }
}