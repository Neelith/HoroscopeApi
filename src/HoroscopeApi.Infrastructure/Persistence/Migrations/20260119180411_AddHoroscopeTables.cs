using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HoroscopeApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHoroscopeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Horoscopes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sign = table.Column<int>(type: "integer", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    GeneralPrediction = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    LovePrediction = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CareerPrediction = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    HealthPrediction = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LuckyNumbers = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LuckyColors = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MoodScore = table.Column<int>(type: "integer", nullable: false),
                    Keywords = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "system"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horoscopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZodiacSigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sign = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    StartMonth = table.Column<int>(type: "integer", nullable: false),
                    StartDay = table.Column<int>(type: "integer", nullable: false),
                    EndMonth = table.Column<int>(type: "integer", nullable: false),
                    EndDay = table.Column<int>(type: "integer", nullable: false),
                    Element = table.Column<int>(type: "integer", nullable: false),
                    Quality = table.Column<int>(type: "integer", nullable: false),
                    Polarity = table.Column<int>(type: "integer", nullable: false),
                    RulingPlanet = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZodiacSigns", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Horoscopes_Sign_Date",
                table: "Horoscopes",
                columns: new[] { "Sign", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Horoscopes_Sign_Period_Date",
                table: "Horoscopes",
                columns: new[] { "Sign", "Period", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ZodiacSigns_Sign",
                table: "ZodiacSigns",
                column: "Sign",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Horoscopes");

            migrationBuilder.DropTable(
                name: "ZodiacSigns");
        }
    }
}
