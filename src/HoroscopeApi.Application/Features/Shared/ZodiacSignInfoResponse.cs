using Hermes.Responses;

namespace HoroscopeApi.Application.Features.Shared;

public sealed record ZodiacSignInfoResponse : IResponse
{
    public required string Name { get; init; }
    public required string Symbol { get; init; }
    public required string Element { get; init; }
    public required string Quality { get; init; }
    public required string Polarity { get; init; }
    public required string RulingPlanet { get; init; }
    public required string DateRange { get; init; }
    public required string Description { get; init; }
}
