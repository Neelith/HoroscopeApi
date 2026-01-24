namespace HoroscopeApi.Application.Features.Shared;

public sealed record HoroscopePredictions
{
    public required string General { get; init; }
    public string? Love { get; init; }
    public string? Career { get; init; }
    public string? Health { get; init; }
}
