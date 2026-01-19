using HoroscopeApi.Shared.Domain;
using Hermes.Results;

namespace HoroscopeApi.Domain.ZodiacSigns;

public sealed class ZodiacSignInfo : Entity
{
    public int Id { get; private set; }
    public ZodiacSign Sign { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;
    public int StartMonth { get; private set; }
    public int StartDay { get; private set; }
    public int EndMonth { get; private set; }
    public int EndDay { get; private set; }
    public Element Element { get; private set; }
    public Quality Quality { get; private set; }
    public Polarity Polarity { get; private set; }
    public string RulingPlanet { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    //EF constructor - required for entity materialization
    private ZodiacSignInfo() { }

    private ZodiacSignInfo(
        ZodiacSign sign,
        string name,
        string symbol,
        int startMonth,
        int startDay,
        int endMonth,
        int endDay,
        Element element,
        Quality quality,
        Polarity polarity,
        string rulingPlanet,
        string description)
    {
        Sign = sign;
        Name = name;
        Symbol = symbol;
        StartMonth = startMonth;
        StartDay = startDay;
        EndMonth = endMonth;
        EndDay = endDay;
        Element = element;
        Quality = quality;
        Polarity = polarity;
        RulingPlanet = rulingPlanet;
        Description = description;
    }

    public static Result<ZodiacSignInfo> Create(
        ZodiacSign sign,
        string name,
        string symbol,
        int startMonth,
        int startDay,
        int endMonth,
        int endDay,
        Element element,
        Quality quality,
        Polarity polarity,
        string rulingPlanet,
        string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Ko<ZodiacSignInfo>(ZodiacSignErrors.InvalidName);
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            return Result.Ko<ZodiacSignInfo>(ZodiacSignErrors.InvalidSymbol);
        }

        if (startMonth < 1 || startMonth > 12 || endMonth < 1 || endMonth > 12)
        {
            return Result.Ko<ZodiacSignInfo>(ZodiacSignErrors.InvalidDateRange);
        }

        var zodiacSignInfo = new ZodiacSignInfo(
            sign,
            name,
            symbol,
            startMonth,
            startDay,
            endMonth,
            endDay,
            element,
            quality,
            polarity,
            rulingPlanet,
            description);

        return Result.Ok(zodiacSignInfo);
    }

    public bool IsDateInRange(int month, int day)
    {
        // Handle special case: Capricorn spans year boundary (Dec 22 - Jan 19)
        if (StartMonth > EndMonth)
        {
            return (month == StartMonth && day >= StartDay) || 
                   (month == EndMonth && day <= EndDay);
        }

        // Normal case: sign within same year
        if (month < StartMonth || month > EndMonth)
        {
            return false;
        }

        if (month == StartMonth && day < StartDay)
        {
            return false;
        }

        if (month == EndMonth && day > EndDay)
        {
            return false;
        }

        return true;
    }
}
