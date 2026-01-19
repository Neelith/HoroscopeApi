using Hermes.Responses;

namespace HoroscopeApi.Application.Features.Shared;

public sealed record ZodiacSignListResponse(List<ZodiacSignInfoResponse> Signs) : IResponse;
