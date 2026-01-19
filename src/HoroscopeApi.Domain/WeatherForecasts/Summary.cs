using System.Text.Json.Serialization;
using ResultExtensions = HoroscopeApi.Shared.Results.ResultExtensions;

namespace HoroscopeApi.Domain.WeatherForecasts;

public record Summary
{
    public string Value { get; private set; }

    [JsonConstructor]
    private Summary(string value)
    {
        Value = value;
    }

    public static Result<Summary> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ResultExtensions.BadRequest<Summary>([SummaryErrors.NullOrEmpty]);
        }

        if (value.Length > 256)
        {
            return ResultExtensions.BadRequest<Summary>([SummaryErrors.SummaryTooLong]);
        }

        return Result.Ok(new Summary(value));
    }
}
