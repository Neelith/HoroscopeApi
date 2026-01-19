using Hermes.Requests;
using HoroscopeApi.Application.Features.Shared;

namespace HoroscopeApi.Application.Features.ZodiacSigns.GetAllZodiacSigns;

public sealed record GetAllZodiacSignsQuery : IQuery<ZodiacSignListResponse>;
