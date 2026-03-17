using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.ZodiacSigns.GetAllZodiacSigns;

public sealed record GetAllZodiacSignsQuery : IQuery<PagedResponse<ZodiacSignData>>;