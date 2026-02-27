using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public static class LuckyNumberGenerator
{
    public static int[] Generate(ZodiacSign sign, DateOnly date)
    {
        int seed = sign.GetHashCode() ^ date.DayNumber;
        Random random = new(seed);

        HashSet<int> numbers = new();
        while (numbers.Count < 3)
        {
            numbers.Add(random.Next(1, 100));
        }

        return numbers.OrderBy(n => n).ToArray();
    }
}