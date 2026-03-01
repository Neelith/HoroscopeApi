namespace HoroscopeApi.Application.Services.ApiKey;

public record ApiKeyValidation(bool IsValid)
{
    public bool IsNotValid => !IsValid;
}