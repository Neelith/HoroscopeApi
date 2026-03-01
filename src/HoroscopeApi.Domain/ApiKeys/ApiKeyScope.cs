using HoroscopeApi.Domain.Shared;

namespace HoroscopeApi.Domain.ApiKeys;

public class ApiKeyScope : AuditableEntity
{
    public int Id { get; set; }
    public required int ApiKeyId { get; set; }
    public required string Name { get; set; }

    //navigation property
    public ApiKey? ApiKey { get; set; }
}