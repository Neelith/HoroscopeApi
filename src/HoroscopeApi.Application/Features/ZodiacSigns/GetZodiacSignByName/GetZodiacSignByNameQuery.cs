using Hermes.Requests;
using Hermes.Responses;
using HoroscopeApi.Application.Features.Shared;

namespace HoroscopeApi.Application.Features.ZodiacSigns.GetZodiacSignByName;

public sealed record GetZodiacSignByNameQuery(string SignName) : IQuery<Response<ZodiacSignInfoData>>;
