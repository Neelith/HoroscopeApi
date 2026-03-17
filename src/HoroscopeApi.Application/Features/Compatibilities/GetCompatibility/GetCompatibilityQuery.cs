using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Compatibilities.GetCompatibility;

public sealed record GetCompatibilityQuery(string FirstSignName, string SecondSignName)
    : IQuery<Response<CompatibilityData>>;