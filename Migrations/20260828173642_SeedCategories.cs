using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sellby.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO "Categories" ("Id", "Name") VALUES
                    ('9f6a1b1e-1a2b-4c3d-8e4f-000000000001', 'Electronics'),
                    ('9f6a1b1e-1a2b-4c3d-8e4f-000000000002', 'Books'),
                    ('9f6a1b1e-1a2b-4c3d-8e4f-000000000003', 'Calculator'),
                    ('9f6a1b1e-1a2b-4c3d-8e4f-000000000004', 'Lab Coat'),
                    ('9f6a1b1e-1a2b-4c3d-8e4f-000000000005', 'Other')
                ON CONFLICT ("Id") DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "Categories" WHERE "Id" IN (
                    '9f6a1b1e-1a2b-4c3d-8e4f-000000000001',
                    '9f6a1b1e-1a2b-4c3d-8e4f-000000000002',
                    '9f6a1b1e-1a2b-4c3d-8e4f-000000000003',
                    '9f6a1b1e-1a2b-4c3d-8e4f-000000000004',
                    '9f6a1b1e-1a2b-4c3d-8e4f-000000000005'
                );
                """);
        }
    }
}
