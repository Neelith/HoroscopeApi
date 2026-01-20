using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoroscopeApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleHoroscopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var today = new DateOnly(2026, 1, 19);
            
            // Seed 7 days of daily horoscopes for all 12 signs
            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                var currentDate = today.AddDays(dayOffset);
                
                // Aries (1)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 1, 1, currentDate, "Today brings exciting opportunities your way. Your natural leadership will shine through in unexpected situations.", "Romance is in the air. Single Aries may meet someone intriguing, while coupled rams should plan something spontaneous.", "A bold move at work could pay off handsomely. Trust your instincts and don't be afraid to take initiative.", "Energy levels are high. Channel it productively through physical activity or creative pursuits.", "[7,14,23]", "[\"red\",\"gold\"]", 8, "[\"confident\",\"energetic\",\"bold\"]", DateTime.UtcNow, "system", false });
                
                // Taurus (2)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 2, 1, currentDate, "Patience and persistence will reward you today. Take time to appreciate the simple pleasures around you.", "Relationships deepen through meaningful conversations. Express your feelings openly and honestly.", "Financial opportunities may arise. Take a practical approach to any new ventures or investments.", "Focus on relaxation and self-care. A spa day or nature walk would be beneficial.", "[6,15,24]", "[\"green\",\"pink\"]", 7, "[\"stable\",\"grounded\",\"patient\"]", DateTime.UtcNow, "system", false });
                
                // Gemini (3)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 3, 1, currentDate, "Communication flows effortlessly today. Your wit and charm will open doors and create connections.", "Flirtation comes naturally. Engage in stimulating conversations that spark intellectual chemistry.", "Networking opportunities abound. Your ideas will be well-received, especially in group settings.", "Mental stimulation is key. Try a new podcast or engaging book to satisfy your curious mind.", "[5,12,21]", "[\"yellow\",\"light blue\"]", 9, "[\"curious\",\"adaptable\",\"social\"]", DateTime.UtcNow, "system", false });
                
                // Cancer (4)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 4, 1, currentDate, "Trust your intuition today. Your emotional intelligence will guide you through any challenges.", "Nurture your relationships with care and attention. Quality time with loved ones brings joy.", "Colleagues appreciate your supportive nature. Your empathy creates a positive work environment.", "Pay attention to emotional well-being. Journaling or meditation can provide clarity and peace.", "[2,11,29]", "[\"silver\",\"white\"]", 6, "[\"intuitive\",\"caring\",\"protective\"]", DateTime.UtcNow, "system", false });
                
                // Leo (5)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 5, 1, currentDate, "Your charisma is magnetic today. Step into the spotlight and let your creativity shine brightly.", "Romance flourishes through grand gestures. Don't hold back on expressing your passion and affection.", "Recognition for your efforts is coming. Your leadership and creativity inspire those around you.", "Vitality is strong. Channel your energy into activities that bring joy and self-expression.", "[1,10,19]", "[\"gold\",\"orange\"]", 9, "[\"confident\",\"creative\",\"generous\"]", DateTime.UtcNow, "system", false });
                
                // Virgo (6)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 6, 1, currentDate, "Attention to detail pays off today. Your analytical skills help solve problems others overlook.", "Show love through thoughtful acts of service. Small gestures make a big impact on relationships.", "Your organizational skills are in high demand. Efficiency and precision lead to professional success.", "Maintain healthy routines. Balance work with adequate rest and nutritious meals.", "[4,13,22]", "[\"navy\",\"grey\"]", 7, "[\"analytical\",\"practical\",\"helpful\"]", DateTime.UtcNow, "system", false });
                
                // Libra (7)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 7, 1, currentDate, "Harmony and balance are within reach. Your diplomatic skills help resolve conflicts gracefully.", "Partnership energy is strong. Compromise and communication strengthen your romantic connections.", "Collaboration brings success. Your ability to see all sides makes you an invaluable team member.", "Seek balance in all things. Yoga or tai chi can help center your mind and body.", "[6,15,24]", "[\"pink\",\"light blue\"]", 8, "[\"diplomatic\",\"harmonious\",\"fair\"]", DateTime.UtcNow, "system", false });
                
                // Scorpio (8)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 8, 1, currentDate, "Intensity and transformation mark this day. Embrace change and let go of what no longer serves you.", "Deep emotional connections are possible. Vulnerability strengthens bonds with your partner.", "Your determination helps you overcome obstacles. Strategic thinking leads to powerful breakthroughs.", "Release emotional tension through physical activity or creative expression. Transformation is healing.", "[3,9,18]", "[\"burgundy\",\"black\"]", 7, "[\"intense\",\"passionate\",\"transformative\"]", DateTime.UtcNow, "system", false });
                
                // Sagittarius (9)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 9, 1, currentDate, "Adventure calls your name. Explore new horizons and expand your philosophical understanding.", "Keep things light and fun in relationships. Your optimism is contagious and attracts positive energy.", "Thinking big brings opportunities. Share your vision and enthusiasm with decision-makers.", "Stay active and explore new physical activities. Outdoor adventures rejuvenate your spirit.", "[3,12,21]", "[\"purple\",\"turquoise\"]", 9, "[\"optimistic\",\"adventurous\",\"philosophical\"]", DateTime.UtcNow, "system", false });
                
                // Capricorn (10)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 10, 1, currentDate, "Discipline and ambition drive success today. Your hard work is building toward impressive achievements.", "Commitment deepens in relationships. Show your loyalty through consistent actions and support.", "Professional recognition is on the horizon. Your reliability and expertise set you apart.", "Structure and routine support wellness. Stick to healthy habits that have proven effective.", "[8,17,26]", "[\"brown\",\"dark green\"]", 7, "[\"ambitious\",\"disciplined\",\"responsible\"]", DateTime.UtcNow, "system", false });
                
                // Aquarius (11)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 11, 1, currentDate, "Innovation and originality set you apart. Your unique perspective offers solutions others haven't considered.", "Independence and connection balance beautifully. Give your partner space while staying emotionally present.", "Creative problem-solving impresses colleagues. Your forward-thinking approach drives progress.", "Mental clarity comes through social connection. Engage with like-minded individuals who inspire you.", "[4,11,22]", "[\"electric blue\",\"silver\"]", 8, "[\"innovative\",\"independent\",\"humanitarian\"]", DateTime.UtcNow, "system", false });
                
                // Pisces (12)
                migrationBuilder.InsertData(
                    table: "Horoscopes",
                    columns: new[] { "ZodiacSignId", "Period", "Date", "GeneralPrediction", "LovePrediction", "CareerPrediction", "HealthPrediction", "LuckyNumbers", "LuckyColors", "MoodScore", "Keywords", "CreatedAtUtc", "CreatedBy", "Deleted" },
                    values: new object[] { 12, 1, currentDate, "Dreams and intuition guide you today. Trust the subtle messages from your subconscious mind.", "Romance feels magical and ethereal. Creative dates and artistic experiences deepen connections.", "Your empathy and creativity are professional assets. Helping others brings career satisfaction.", "Prioritize rest and spiritual practices. Meditation, art, or music soothes your sensitive soul.", "[7,16,25]", "[\"sea green\",\"lavender\"]", 6, "[\"compassionate\",\"intuitive\",\"artistic\"]", DateTime.UtcNow, "system", false });
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var today = new DateOnly(2026, 1, 19);
            
            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                var currentDate = today.AddDays(dayOffset);
                
                for (int sign = 1; sign <= 12; sign++)
                {
                    migrationBuilder.DeleteData(
                        table: "Horoscopes",
                        keyColumns: new[] { "ZodiacSignId", "Period", "Date" },
                        keyValues: new object[] { sign, 1, currentDate });
                }
            }
        }
    }
}
