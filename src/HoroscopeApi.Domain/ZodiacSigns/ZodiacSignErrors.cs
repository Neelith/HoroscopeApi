using Hermes.Results;

namespace HoroscopeApi.Domain.ZodiacSigns;

public static class ZodiacSignErrors
{
    public static Error NotFound(ZodiacSign sign) => new(
        "ZodiacSign.NotFound",
        $"Zodiac sign '{sign}' was not found.");

    public static Error NotFoundByDate => new(
        "ZodiacSign.NotFoundByDate",
        "No zodiac sign found for the provided date.");

    public static Error InvalidName => new(
        "ZodiacSign.InvalidName",
        "Zodiac sign name is required.");

    public static Error InvalidSymbol => new(
        "ZodiacSign.InvalidSymbol",
        "Zodiac sign symbol is required.");

    public static Error InvalidDateRange => new(
        "ZodiacSign.InvalidDateRange",
        "Invalid date range provided for zodiac sign.");

    public static Error InvalidDate => new(
        "ZodiacSign.InvalidDate",
        "Invalid date provided for zodiac sign lookup.");
}
