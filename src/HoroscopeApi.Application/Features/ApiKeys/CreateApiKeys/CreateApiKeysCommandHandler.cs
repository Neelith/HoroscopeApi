using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Infrastructure.User;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Domain.ApiKeys;
using HoroscopeApi.Domain.ApiKeys.Repositories;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;

internal sealed class CreateApiKeysCommandHandler(
    ILogger<CreateApiKeysCommandHandler> logger,
    IApiKeyRepository apiKeyRepository,
    IUnitOfWork unitOfWork,
    IApiKeyService apiKeyService,
    ICurrentUserService currentUserService)
    : ICommandHandler<CreateApiKeysCommands, PagedResponse<CreateApiKeyData>>
{
    private const int PrefixLength = 8;
    private const string Algorithm = "HMAC-SHA256";

    public async Task<Result<PagedResponse<CreateApiKeyData>>> Handle(
        CreateApiKeysCommands commands,
        CancellationToken cancellationToken)
    {
        CurrentUser currentUser = currentUserService.GetCurrentUser();

        List<(ApiKey Entity, string PlainTextKey)> created = [];

        foreach (CreateApiKeyCommand item in commands.Commands)
        {
            string plainTextKey = apiKeyService.GeneratePlainTextKey();
            string salt = apiKeyService.GenerateSalt();
            string hash = apiKeyService.ComputeHash(plainTextKey, salt);
            string prefix = plainTextKey[..PrefixLength];

            ApiKey apiKey = new()
            {
                OwnerId = currentUser.Id,
                Prefix = prefix,
                Hash = hash,
                Salt = salt,
                Algorithm = Algorithm,
                Type = item.Type,
                RateLimitType = item.RateLimitType,
                RateLimitCount = item.RateLimitCount,
                RateLimit = item.RateLimit,
                Scopes = item.Scopes?.Select(name => new ApiKeyScope { ApiKeyId = 0, Name = name }).ToList() ?? []
            };

            created.Add((apiKey, plainTextKey));
        }

        List<ApiKey> entities = created.Select(c => c.Entity).ToList();
        UpsertApiKeysRepositoryCommand repositoryCommand = new(entities);
        await apiKeyRepository.UpsertRangeAsync(repositoryCommand, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save API keys");
            return Result.Ko<PagedResponse<CreateApiKeyData>>(ApiKeyErrors.CreationFailed);
        }

        List<CreateApiKeyData> dataList = created
            .Select(c => CreateApiKeyData.ToData(c.Entity, c.PlainTextKey))
            .ToList();

        PagedResponse<CreateApiKeyData> response = PagedResponse<CreateApiKeyData>.Create(dataList, dataList.Count);
        return Result.Ok(response);
    }
}