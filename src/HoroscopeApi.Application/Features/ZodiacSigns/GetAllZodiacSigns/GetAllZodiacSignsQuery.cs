using Hermes.Requests;
using Hermes.Responses;
using HoroscopeApi.Application.Features.Shared;

namespace HoroscopeApi.Application.Features.ZodiacSigns.GetAllZodiacSigns;

public sealed record GetAllZodiacSignsQuery : IQuery<PagedResponse<ZodiacSignInfoData>>;
