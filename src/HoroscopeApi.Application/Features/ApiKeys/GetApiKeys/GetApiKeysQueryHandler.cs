using HoroscopeApi.Application.Infrastructure.User;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.ApiKeys;
using HoroscopeApi.Domain.ApiKeys.Repositories;

namespace HoroscopeApi.Application.Features.ApiKeys.GetApiKeys;

internal sealed class GetApiKeysQueryHandler(IApiKeyRepository repository, ICurrentUserService currentUserService)
    : IQueryHandler<GetApiKeysQuery, PagedResponse<ApiKeyData>>
{
    public async Task<Result<PagedResponse<ApiKeyData>>> Handle(
        GetApiKeysQuery query,
        CancellationToken cancellationToken)
    {
        CurrentUser currentUser = currentUserService.GetCurrentUser();

        List<int>? ids = null;
        if (!string.IsNullOrWhiteSpace(query.Ids))
        {
            ids = query.Ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse)
                .ToList();
        }

        GetApiKeysByFilterRepositoryQuery repositoryQuery = new(
            OwnerId: currentUser.Id,
            Type: query.Type,
            RateLimitType: query.RateLimitType,
            Ids: ids);

        List<ApiKey> apiKeys = await repository.GetByFilterAsync(repositoryQuery, cancellationToken);

        List<ApiKeyData> dataList = apiKeys.Select(ApiKeyData.FromEntity).ToList();

        PagedResponse<ApiKeyData> response = PagedResponse<ApiKeyData>.Create(dataList, dataList.Count);
        return Result.Ok(response);
    }
}