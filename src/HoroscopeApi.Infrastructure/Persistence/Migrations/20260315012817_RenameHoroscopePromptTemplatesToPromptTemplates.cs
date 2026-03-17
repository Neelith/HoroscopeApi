using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoroscopeApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameHoroscopePromptTemplatesToPromptTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "HoroscopePromptTemplates",
                newName: "PromptTemplates");

            migrationBuilder.RenameColumn(
                name: "Period",
                table: "PromptTemplates",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_HoroscopePromptTemplates_Period",
                table: "PromptTemplates",
                newName: "IX_PromptTemplates_Type");

            // Rename primary key constraint
            migrationBuilder.Sql(
                """ALTER TABLE "PromptTemplates" RENAME CONSTRAINT "PK_HoroscopePromptTemplates" TO "PK_PromptTemplates";""");

            // Seed compatibility prompt template (Type = 5)
            SeedCompatibilityPromptTemplate(migrationBuilder);
        }

        private static void SeedCompatibilityPromptTemplate(MigrationBuilder migrationBuilder)
        {
            const string systemPrompt =
                "You are an expert astrologer specializing in zodiac sign compatibility analysis. " +
                "Generate a compatibility assessment in JSON format that is insightful, balanced, and meaningful. " +
                "Analyze the relationship dynamics between the two signs including strengths, challenges, and overall harmony. " +
                "Provide a compatibility score from 0 to 100.\n\n" +
                "IMPORTANT: Return ONLY a single flat JSON object. Do NOT wrap it in an array.\n\n" +
                "Required JSON structure:\n" +
                "{\n" +
                "  \"firstSign\": \"aries\",\n" +
                "  \"secondSign\": \"leo\",\n" +
                "  \"score\": 85,\n" +
                "  \"description\": \"...\"\n" +
                "}";

            const string fewShotExamples = """
[{"role":"user","content":"Analyze the compatibility between aries and leo.\n\nAries: Fire element, Cardinal quality, ruled by Mars. The Ram - bold, pioneering, and energetic.\nLeo: Fire element, Fixed quality, ruled by Sun. The Lion - confident, dramatic, and warm-hearted.\n\nReturn a single JSON object (not an array)."},{"role":"assistant","content":"{\n  \"firstSign\": \"aries\",\n  \"secondSign\": \"leo\",\n  \"score\": 88,\n  \"description\": \"Aries and Leo form one of the most dynamic and passionate pairings in the zodiac. As fellow Fire signs, they share an innate understanding of each other's need for excitement, adventure, and self-expression. Aries' bold pioneering spirit complements Leo's confident and warm-hearted nature, creating a relationship filled with mutual admiration and enthusiasm. Mars and the Sun make a powerful planetary combination, fueling both physical attraction and intellectual stimulation. Their shared element creates instant chemistry and a natural sense of camaraderie. Challenges may arise from their equally strong personalities — Aries' impulsive nature can clash with Leo's desire for loyalty and consistency, and both signs have a competitive streak that needs careful management. However, their mutual respect for courage and authenticity helps them navigate conflicts with honesty and directness. When these two signs learn to take turns in the spotlight and channel their fiery energy into shared goals, they become an unstoppable team. Their relationship thrives on spontaneity, grand gestures, and a deep appreciation for each other's strengths.\"\n}"}]
""";

            const string userPromptTemplate =
                "Analyze the compatibility between {firstSignName} and {secondSignName}.\n\n" +
                "{firstSignName}: {firstElement} element, {firstQuality} quality, ruled by {firstRulingPlanet}. {firstDescription}\n" +
                "{secondSignName}: {secondElement} element, {secondQuality} quality, ruled by {secondRulingPlanet}. {secondDescription}\n\n" +
                "Return a single JSON object (not an array).";

            migrationBuilder.Sql($"""
                INSERT INTO "PromptTemplates" ("Type", "SystemPrompt", "FewShotExamples", "UserPromptTemplate", "Deleted", "CreatedBy", "CreatedAtUtc")
                VALUES (5, '{EscapeSql(systemPrompt)}', '{EscapeSql(fewShotExamples.Trim())}', '{EscapeSql(userPromptTemplate)}', false, 'migration', CURRENT_TIMESTAMP);
                """);
        }

        private static string EscapeSql(string value)
        {
            return value.Replace("'", "''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove compatibility prompt template
            migrationBuilder.Sql("""DELETE FROM "PromptTemplates" WHERE "Type" = 5;""");

            migrationBuilder.RenameTable(
                name: "PromptTemplates",
                newName: "HoroscopePromptTemplates");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "HoroscopePromptTemplates",
                newName: "Period");

            migrationBuilder.RenameIndex(
                name: "IX_PromptTemplates_Type",
                table: "HoroscopePromptTemplates",
                newName: "IX_HoroscopePromptTemplates_Period");

            migrationBuilder.Sql(
                """ALTER TABLE "HoroscopePromptTemplates" RENAME CONSTRAINT "PK_PromptTemplates" TO "PK_HoroscopePromptTemplates";""");
        }
    }
}
