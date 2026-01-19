using System.Collections.Generic;

namespace HoroscopeApi.Application.Features.Shared;

public sealed record HoroscopeResponse : IResponse
{
    public required string Sign { get; init; }
    public required ZodiacSignInfoResponse SignInfo { get; init; }
    public required string Period { get; init; }
    public required DateOnly Date { get; init; }
    public required HoroscopePredictionsResponse Predictions { get; init; }
    public required List<int> LuckyNumbers { get; init; }
    public required List<string> LuckyColors { get; init; }
    public required int MoodScore { get; init; }
    public required List<string> Keywords { get; init; }
}
