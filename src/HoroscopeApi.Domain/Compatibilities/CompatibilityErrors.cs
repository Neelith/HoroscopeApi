using HoroscopeApi.Domain.Constants;

namespace HoroscopeApi.Domain.Compatibilities;

public static class CompatibilityErrors
{
    public static Error NotFound => new(
        "Compatibility.NotFound",
        "No compatibility result found for the specified sign pair.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };

    public static Error InvalidScore => new(
        "Compatibility.InvalidScore",
        "Compatibility score must be between 0 and 100.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };

    public static Error InvalidDescription => new(
        "Compatibility.InvalidDescription",
        "Compatibility description is required and cannot be empty.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };
}