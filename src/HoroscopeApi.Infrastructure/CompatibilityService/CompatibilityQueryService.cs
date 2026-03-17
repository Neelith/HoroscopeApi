using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Application.Services.CompatibilityService;
using HoroscopeApi.Domain.Compatibilities;
using HoroscopeApi.Domain.Compatibilities.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Infrastructure.CompatibilityService;

internal sealed class CompatibilityQueryService(
    ILogger<CompatibilityQueryService> logger,
    ICompatibilityRepository compatibilityRepository,
    ICompatibilityGeneratorService compatibilityGeneratorService,
    IZodiacSignRepository zodiacSignRepository,
    IUnitOfWork unitOfWork,
    IRedisCache cache) : ICompatibilityQueryService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public async Task<Result<(CompatibilityData Data, bool IsCached)>> GetOrGenerateCompatibilityAsync(
        ZodiacSign firstSign,
        ZodiacSign secondSign,
        CancellationToken cancellationToken)
    {
        // Normalize pair: lower enum value first
        (ZodiacSign normalizedFirst, ZodiacSign normalizedSecond) = NormalizeSignPair(firstSign, secondSign);

        // Try cache first
        string cacheKey = $"compatibility:{normalizedFirst}:{normalizedSecond}";
        CompatibilityData? cachedData = await cache.GetAsync<CompatibilityData>(cacheKey, cancellationToken);
        if (cachedData is not null)
        {
            return Result.Ok((cachedData, true));
        }

        // Fetch zodiac sign info for both signs
        ZodiacSignInfo? firstSignInfo = await zodiacSignRepository.GetBySignAsync(
            new GetZodiacSignBySignRepositoryQuery(normalizedFirst), cancellationToken);
        ZodiacSignInfo? secondSignInfo = await zodiacSignRepository.GetBySignAsync(
            new GetZodiacSignBySignRepositoryQuery(normalizedSecond), cancellationToken);

        if (firstSignInfo is null)
        {
            return Result.Ko<(CompatibilityData, bool)>(ZodiacSignErrors.NotFound(normalizedFirst));
        }

        if (secondSignInfo is null)
        {
            return Result.Ko<(CompatibilityData, bool)>(ZodiacSignErrors.NotFound(normalizedSecond));
        }

        // Try database
        GetCompatibilityBySignPairRepositoryQuery repositoryQuery = new(firstSignInfo.Id, secondSignInfo.Id);
        Compatibility? compatibility =
            await compatibilityRepository.GetBySignPairAsync(repositoryQuery, cancellationToken);

        if (compatibility is null)
        {
            Result<Compatibility?> generateResult = await GenerateCompatibilityUsingAiAsync(
                firstSignInfo, secondSignInfo, repositoryQuery, cancellationToken);

            if (generateResult.IsFailure)
            {
                logger.LogError(
                    "Failed to generate compatibility for signs {FirstSign} and {SecondSign}: {@Errors}",
                    normalizedFirst, normalizedSecond, generateResult.Errors);
                return Result.Ko<(CompatibilityData, bool)>(CompatibilityErrors.NotFound);
            }

            compatibility = generateResult.Value!;
        }

        CompatibilityData data = CompatibilityData.ToCompatibilityData(compatibility);

        await cache.SetAsync(cacheKey, data, CacheTtl, cancellationToken);

        return Result.Ok((data, false));
    }

    private async Task<Result<Compatibility?>> GenerateCompatibilityUsingAiAsync(
        ZodiacSignInfo firstSignInfo,
        ZodiacSignInfo secondSignInfo,
        GetCompatibilityBySignPairRepositoryQuery repositoryQuery,
        CancellationToken cancellationToken)
    {
        Result<HuggingFaceCompatibilityData> generationResult =
            await compatibilityGeneratorService.GenerateCompatibilityAsync(
                firstSignInfo, secondSignInfo, cancellationToken);

        if (!generationResult.IsSuccess)
        {
            return Result.Ko<Compatibility?>(generationResult.Errors);
        }

        HuggingFaceCompatibilityData aiData = generationResult.Value!;

        Result<Compatibility> compatibilityResult = Compatibility.Create(
            firstSignInfo.Id,
            secondSignInfo.Id,
            aiData.Score,
            aiData.Description);

        if (!compatibilityResult.IsSuccess)
        {
            return Result.Ko<Compatibility?>(compatibilityResult.Errors);
        }

        Compatibility compatibility = compatibilityResult.Value!;

        await compatibilityRepository.AddAsync(compatibility, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload to populate navigation properties
        compatibility = (await compatibilityRepository.GetBySignPairAsync(repositoryQuery, cancellationToken))!;

        return compatibility;
    }

    private static (ZodiacSign First, ZodiacSign Second) NormalizeSignPair(ZodiacSign a, ZodiacSign b)
    {
        return (int)a <= (int)b ? (a, b) : (b, a);
    }
}