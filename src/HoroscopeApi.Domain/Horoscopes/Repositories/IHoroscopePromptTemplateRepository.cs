namespace HoroscopeApi.Domain.Horoscopes.Repositories;

public interface IHoroscopePromptTemplateRepository
{
    Task<HoroscopePromptTemplate?> GetByPeriodAsync(
        HoroscopePeriod period,
        CancellationToken cancellationToken);
}
