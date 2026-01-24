namespace HoroscopeApi.Application.Features.Shared;

public sealed record HoroscopeData
{
    public required string Sign { get; init; }
    public required ZodiacSignInfoData SignInfo { get; init; }
    public required string Period { get; init; }
    public required DateOnly Date { get; init; }
    public required HoroscopePredictions Predictions { get; init; }
    public required List<int> LuckyNumbers { get; init; }
    public required List<string> LuckyColors { get; init; }
    public required int MoodScore { get; init; }
    public required List<string> Keywords { get; init; }
}
