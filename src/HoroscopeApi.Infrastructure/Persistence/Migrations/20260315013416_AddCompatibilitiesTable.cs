using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HoroscopeApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompatibilitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Compatibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstZodiacSignId = table.Column<int>(type: "integer", nullable: false),
                    SecondZodiacSignId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compatibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compatibilities_ZodiacSigns_FirstZodiacSignId",
                        column: x => x.FirstZodiacSignId,
                        principalTable: "ZodiacSigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Compatibilities_ZodiacSigns_SecondZodiacSignId",
                        column: x => x.SecondZodiacSignId,
                        principalTable: "ZodiacSigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Compatibilities_FirstZodiacSignId_SecondZodiacSignId",
                table: "Compatibilities",
                columns: new[] { "FirstZodiacSignId", "SecondZodiacSignId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compatibilities_SecondZodiacSignId",
                table: "Compatibilities",
                column: "SecondZodiacSignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Compatibilities");
        }
    }
}
