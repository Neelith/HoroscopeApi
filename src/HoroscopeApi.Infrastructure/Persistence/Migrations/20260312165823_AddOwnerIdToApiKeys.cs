using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoroscopeApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerIdToApiKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "ApiKeys",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OwnerId",
                table: "ApiKeys",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_OwnerId",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ApiKeys");
        }
    }
}
