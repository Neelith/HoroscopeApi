using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoroscopeApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceSignWithZodiacSignRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add new ZodiacSignId column (nullable temporarily)
            migrationBuilder.AddColumn<int>(
                name: "ZodiacSignId",
                table: "Horoscopes",
                type: "integer",
                nullable: true);

            // Step 2: Populate ZodiacSignId from existing Sign enum values
            // Map Sign enum values to ZodiacSignInfo.Id by matching the Sign enum
            migrationBuilder.Sql(@"
                UPDATE ""Horoscopes"" h
                SET ""ZodiacSignId"" = z.""Id""
                FROM ""ZodiacSigns"" z
                WHERE z.""Sign"" = h.""Sign"";
            ");

            // Step 3: Make ZodiacSignId NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "ZodiacSignId",
                table: "Horoscopes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // Step 4: Drop old indexes on Sign
            migrationBuilder.DropIndex(
                name: "IX_Horoscopes_Sign_Date",
                table: "Horoscopes");

            migrationBuilder.DropIndex(
                name: "IX_Horoscopes_Sign_Period_Date",
                table: "Horoscopes");

            // Step 5: Remove old Sign column
            migrationBuilder.DropColumn(
                name: "Sign",
                table: "Horoscopes");

            // Step 6: Create new indexes on ZodiacSignId
            migrationBuilder.CreateIndex(
                name: "IX_Horoscopes_ZodiacSignId_Date",
                table: "Horoscopes",
                columns: new[] { "ZodiacSignId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Horoscopes_ZodiacSignId_Period_Date",
                table: "Horoscopes",
                columns: new[] { "ZodiacSignId", "Period", "Date" });

            // Step 7: Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Horoscopes_ZodiacSigns_ZodiacSignId",
                table: "Horoscopes",
                column: "ZodiacSignId",
                principalTable: "ZodiacSigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Horoscopes_ZodiacSigns_ZodiacSignId",
                table: "Horoscopes");

            // Step 2: Drop indexes on ZodiacSignId
            migrationBuilder.DropIndex(
                name: "IX_Horoscopes_ZodiacSignId_Date",
                table: "Horoscopes");

            migrationBuilder.DropIndex(
                name: "IX_Horoscopes_ZodiacSignId_Period_Date",
                table: "Horoscopes");

            // Step 3: Add back Sign column (nullable temporarily)
            migrationBuilder.AddColumn<int>(
                name: "Sign",
                table: "Horoscopes",
                type: "integer",
                nullable: true);

            // Step 4: Populate Sign from ZodiacSignId
            migrationBuilder.Sql(@"
                UPDATE ""Horoscopes"" h
                SET ""Sign"" = z.""Sign""
                FROM ""ZodiacSigns"" z
                WHERE z.""Id"" = h.""ZodiacSignId"";
            ");

            // Step 5: Make Sign NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "Sign",
                table: "Horoscopes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // Step 6: Remove ZodiacSignId column
            migrationBuilder.DropColumn(
                name: "ZodiacSignId",
                table: "Horoscopes");

            // Step 7: Recreate old indexes on Sign
            migrationBuilder.CreateIndex(
                name: "IX_Horoscopes_Sign_Date",
                table: "Horoscopes",
                columns: new[] { "Sign", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Horoscopes_Sign_Period_Date",
                table: "Horoscopes",
                columns: new[] { "Sign", "Period", "Date" });
        }
    }
}
