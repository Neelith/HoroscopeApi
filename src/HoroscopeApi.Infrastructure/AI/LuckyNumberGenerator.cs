using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public static class LuckyNumberGenerator
{
    public static int[] Generate(ZodiacSign sign, DateOnly date)
    {
        int seed = sign.GetHashCode() ^ date.DayNumber;
        var random = new Random(seed);
        
        var numbers = new HashSet<int>();
        while (numbers.Count < 3)
        {
            numbers.Add(random.Next(1, 100));
        }
        
        return numbers.OrderBy(n => n).ToArray();
    }
}
