using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoroscopeApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNameAndRemoveRateLimitCountFromApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RateLimitCount",
                table: "ApiKeys");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ApiKeys",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ApiKeys");

            migrationBuilder.AddColumn<int>(
                name: "RateLimitCount",
                table: "ApiKeys",
                type: "integer",
                nullable: true);
        }
    }
}
