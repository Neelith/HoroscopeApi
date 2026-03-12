namespace HoroscopeApi.Application.Infrastructure.User;

public sealed record CurrentUser
{
    public required Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
}
