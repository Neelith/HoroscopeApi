using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoroscopeApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixHoroscopeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Horoscopes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldDefaultValue: "system");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Horoscopes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "system",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "");
        }
    }
}
