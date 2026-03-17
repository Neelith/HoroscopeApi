using HoroscopeApi.Domain.Shared;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Domain.Compatibilities;

public sealed class Compatibility : AuditableEntity
{
    //EF constructor - required for entity materialization
    private Compatibility() { }

    private Compatibility(
        int firstZodiacSignId,
        int secondZodiacSignId,
        int score,
        string description)
    {
        FirstZodiacSignId = firstZodiacSignId;
        SecondZodiacSignId = secondZodiacSignId;
        Score = score;
        Description = description;
    }

    public int Id { get; private set; }
    public int FirstZodiacSignId { get; private set; }
    public ZodiacSignInfo FirstZodiacSignInfo { get; private set; } = null!;
    public int SecondZodiacSignId { get; private set; }
    public ZodiacSignInfo SecondZodiacSignInfo { get; private set; } = null!;
    public int Score { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public static Result<Compatibility> Create(
        int firstZodiacSignId,
        int secondZodiacSignId,
        int score,
        string description)
    {
        if (score < 0 || score > 100)
        {
            return Result.Ko<Compatibility>(CompatibilityErrors.InvalidScore);
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result.Ko<Compatibility>(CompatibilityErrors.InvalidDescription);
        }

        Compatibility compatibility = new(
            firstZodiacSignId,
            secondZodiacSignId,
            score,
            description);

        return Result.Ok(compatibility);
    }
}