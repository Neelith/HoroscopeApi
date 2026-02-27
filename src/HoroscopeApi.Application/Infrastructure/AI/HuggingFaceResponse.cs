using System.Text.Json.Serialization;

namespace HoroscopeApi.Application.Infrastructure.AI;

public sealed record HuggingFaceHoroscopeData(
    [property: JsonPropertyName("sign")] string Sign,
    [property: JsonPropertyName("general")]
    string General,
    [property: JsonPropertyName("love")] string Love,
    [property: JsonPropertyName("career")] string Career,
    [property: JsonPropertyName("health")] string Health,
    [property: JsonPropertyName("keywords")]
    List<string> Keywords,
    [property: JsonPropertyName("luckyColors")]
    List<string> LuckyColors,
    [property: JsonPropertyName("moodScore")]
    int MoodScore);