using System.Text.Json.Serialization;

namespace HoroscopeApi.Application.Services.AI;

public sealed record HuggingFaceCompatibilityData(
    [property: JsonPropertyName("firstSign")]
    string FirstSign,
    [property: JsonPropertyName("secondSign")]
    string SecondSign,
    [property: JsonPropertyName("score")] int Score,
    [property: JsonPropertyName("description")]
    string Description);