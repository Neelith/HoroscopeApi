using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoroscopeApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToJsonbArrays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add temporary JSONB columns
            migrationBuilder.Sql(@"
                ALTER TABLE ""Horoscopes"" ADD COLUMN ""LuckyNumbers_temp"" jsonb;
                ALTER TABLE ""Horoscopes"" ADD COLUMN ""LuckyColors_temp"" jsonb;
                ALTER TABLE ""Horoscopes"" ADD COLUMN ""Keywords_temp"" jsonb;
            ");

            // Step 2: Convert existing data from VARCHAR to JSONB
            // Data is already in JSON format (e.g., '[1,2,3]'), so we can cast it directly
            migrationBuilder.Sql(@"
                UPDATE ""Horoscopes""
                SET ""LuckyNumbers_temp"" = ""LuckyNumbers""::jsonb;
                
                UPDATE ""Horoscopes""
                SET ""LuckyColors_temp"" = ""LuckyColors""::jsonb;
                
                UPDATE ""Horoscopes""
                SET ""Keywords_temp"" = ""Keywords""::jsonb;
            ");

            // Step 3: Drop old VARCHAR columns
            migrationBuilder.Sql(@"
                ALTER TABLE ""Horoscopes"" DROP COLUMN ""LuckyNumbers"";
                ALTER TABLE ""Horoscopes"" DROP COLUMN ""LuckyColors"";
                ALTER TABLE ""Horoscopes"" DROP COLUMN ""Keywords"";
            ");

            // Step 4: Rename temporary columns to original names
            migrationBuilder.Sql(@"
                ALTER TABLE ""Horoscopes"" RENAME COLUMN ""LuckyNumbers_temp"" TO ""LuckyNumbers"";
                ALTER TABLE ""Horoscopes"" RENAME COLUMN ""LuckyColors_temp"" TO ""LuckyColors"";
                ALTER TABLE ""Horoscopes"" RENAME COLUMN ""Keywords_temp"" TO ""Keywords"";
            ");

            // Step 5: Add NOT NULL constraints
            migrationBuilder.Sql(@"
                ALTER TABLE ""Horoscopes"" ALTER COLUMN ""LuckyNumbers"" SET NOT NULL;
                ALTER TABLE ""Horoscopes"" ALTER COLUMN ""LuckyColors"" SET NOT NULL;
                ALTER TABLE ""Horoscopes"" ALTER COLUMN ""Keywords"" SET NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add temporary VARCHAR columns
            migrationBuilder.Sql(@"
                ALTER TABLE ""Horoscopes"" ADD COLUMN ""LuckyNumbers_temp"" character varying(100);
                ALTER TABLE ""Horoscopes"" ADD COLUMN ""LuckyColors_temp"" character varying(200);
                ALTER TABLE ""Horoscopes"" ADD COLUMN ""Keywords_temp"" character varying(500);
            ");

            // Step 2: Convert JSONB back to VARCHAR (cast to text)
            migrationBuilder.Sql(@"
                UPDATE ""Horoscopes""
                SET ""LuckyNumbers_temp"" = ""LuckyNumbers""::text;
                
                UPDATE ""Horoscopes""
                SET ""LuckyColors_temp"" = ""LuckyColors""::text;
                
                UPDATE ""Horoscopes""
                SET ""Keywords_temp"" = ""Keywords""::text;
            ");

            // Step 3: Drop JSONB columns
            migrationBuilder.Sql(@"
                ALTER TABLE ""Horoscopes"" DROP COLUMN ""LuckyNumbers"";
                ALTER TABLE ""Horoscopes"" DROP COLUMN ""LuckyColors"";
                ALTER TABLE ""Horoscopes"" DROP COLUMN ""Keywords"";
            ");

            // Step 4: Rename temporary columns back
            migrationBuilder.Sql(@"
                ALTER TABLE ""Horoscopes"" RENAME COLUMN ""LuckyNumbers_temp"" TO ""LuckyNumbers"";
                ALTER TABLE ""Horoscopes"" RENAME COLUMN ""LuckyColors_temp"" TO ""LuckyColors"";
                ALTER TABLE ""Horoscopes"" RENAME COLUMN ""Keywords_temp"" TO ""Keywords"";
            ");

            // Step 5: Add NOT NULL constraints
            migrationBuilder.Sql(@"
                ALTER TABLE ""Horoscopes"" ALTER COLUMN ""LuckyNumbers"" SET NOT NULL;
                ALTER TABLE ""Horoscopes"" ALTER COLUMN ""LuckyColors"" SET NOT NULL;
                ALTER TABLE ""Horoscopes"" ALTER COLUMN ""Keywords"" SET NOT NULL;
            ");
        }
    }
}
