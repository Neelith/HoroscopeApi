using Hermes.Results;
using HoroscopeApi.Shared.Constants;

namespace HoroscopeApi.Domain.ZodiacSigns;

public static class ZodiacSignErrors
{
    public static Error NotFound(ZodiacSign sign) => new(
        "ZodiacSign.NotFound",
        $"Zodiac sign '{sign}' was not found.")
    {
        Metadata = new Dictionary<string, string?>
        {
            { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode }
        }
    };

    public static Error NotFoundByDate => new(
        "ZodiacSign.NotFoundByDate",
        "No zodiac sign found for the provided date.")
    {
        Metadata = new Dictionary<string, string?>
        {
            { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode }
        }
    };

    public static Error InvalidName => new(
        "ZodiacSign.InvalidName",
        "Zodiac sign name is required.")
    {
        Metadata = new Dictionary<string, string?>
        {
            { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode }
        }
    };

    public static Error InvalidSymbol => new(
        "ZodiacSign.InvalidSymbol",
        "Zodiac sign symbol is required.")
    {
        Metadata = new Dictionary<string, string?>
        {
            { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode }
        }
    };

    public static Error InvalidDateRange => new(
        "ZodiacSign.InvalidDateRange",
        "Invalid date range provided for zodiac sign.")
    {
        Metadata = new Dictionary<string, string?>
        {
            { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode }
        }
    };

    public static Error InvalidDate => new(
        "ZodiacSign.InvalidDate",
        "Invalid date provided for zodiac sign lookup.")
    {
        Metadata = new Dictionary<string, string?>
        {
            { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode }
        }
    };
}
