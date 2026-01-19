using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoroscopeApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedZodiacSigns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ZodiacSigns",
                columns: new[] { "Sign", "Name", "Symbol", "StartMonth", "StartDay", "EndMonth", "EndDay", "Element", "Quality", "Polarity", "RulingPlanet", "Description" },
                values: new object[,]
                {
                    { 1, "Aries", "♈", 3, 21, 4, 19, 1, 1, 1, "Mars", "The Ram - Bold, ambitious, and passionate. Natural leaders who embrace challenges with courage and determination." },
                    { 2, "Taurus", "♉", 4, 20, 5, 20, 2, 2, 2, "Venus", "The Bull - Patient, reliable, and devoted. Value stability, comfort, and the finer things in life." },
                    { 3, "Gemini", "♊", 5, 21, 6, 20, 3, 3, 1, "Mercury", "The Twins - Adaptable, outgoing, and intelligent. Quick-witted communicators who thrive on variety and mental stimulation." },
                    { 4, "Cancer", "♋", 6, 21, 7, 22, 4, 1, 2, "Moon", "The Crab - Intuitive, emotional, and protective. Deeply caring souls who value home, family, and emotional security." },
                    { 5, "Leo", "♌", 7, 23, 8, 22, 1, 2, 1, "Sun", "The Lion - Confident, creative, and generous. Natural performers who radiate warmth and magnetic charisma." },
                    { 6, "Virgo", "♍", 8, 23, 9, 22, 2, 3, 2, "Mercury", "The Maiden - Analytical, practical, and meticulous. Detail-oriented perfectionists dedicated to service and improvement." },
                    { 7, "Libra", "♎", 9, 23, 10, 22, 3, 1, 1, "Venus", "The Scales - Diplomatic, gracious, and fair-minded. Seekers of harmony, balance, and beauty in all aspects of life." },
                    { 8, "Scorpio", "♏", 10, 23, 11, 21, 4, 2, 2, "Pluto", "The Scorpion - Passionate, resourceful, and intense. Deeply transformative souls with powerful emotional depth." },
                    { 9, "Sagittarius", "♐", 11, 22, 12, 21, 1, 3, 1, "Jupiter", "The Archer - Optimistic, adventurous, and philosophical. Free spirits seeking truth, wisdom, and new experiences." },
                    { 10, "Capricorn", "♑", 12, 22, 1, 19, 2, 1, 2, "Saturn", "The Goat - Disciplined, ambitious, and responsible. Patient achievers who build lasting success through dedication." },
                    { 11, "Aquarius", "♒", 1, 20, 2, 18, 3, 2, 1, "Uranus", "The Water Bearer - Progressive, original, and humanitarian. Visionary thinkers who champion innovation and social change." },
                    { 12, "Pisces", "♓", 2, 19, 3, 20, 4, 3, 2, "Neptune", "The Fish - Compassionate, artistic, and intuitive. Dreamy souls with deep empathy and creative imagination." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ZodiacSigns",
                keyColumn: "Sign",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
        }
    }
}
