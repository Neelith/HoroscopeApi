namespace HoroscopeApi.WebApi.Infrastructure.Setup.Extensions;

internal static class AddSettingsExtension
{
    public static TSettings? AddSettings<TSettings>(this IServiceCollection services, IConfiguration configuration,
        ILogger startupLogger) where TSettings : class, new()
    {
        IConfigurationSection settingsSection = configuration.GetSection(typeof(TSettings).Name);

        if (!settingsSection.Exists())
        {
            startupLogger.LogWarning("Settings section {SettingsName} not found in configuration",
                typeof(TSettings).Name);
            return null;
        }

        services.Configure<TSettings>(settingsSection);

        return settingsSection.Get<TSettings>();
    }
}