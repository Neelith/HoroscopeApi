using System.Text.Json.Serialization;

namespace HoroscopeApi.Application.Services.AI;

public sealed record ChatMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")]
    string Content);