namespace HoroscopeApi.Domain.Compatibilities.Repositories;

public sealed record GetCompatibilityBySignPairRepositoryQuery(
    int FirstZodiacSignId,
    int SecondZodiacSignId);