using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.ZodiacSigns.GetZodiacSignByName;

public sealed record GetZodiacSignByNameQuery(string SignName) : IQuery<Response<ZodiacSignData>>;