using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class ValidateProfileSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Run validation SQL
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'JobTitleId' AND Object_ID = Object_ID(N'Profiles'))
                BEGIN
                    THROW 51000, 'JobTitleId column does not exist in Profiles table. Schema validation failed.', 1;
                END

                IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'JobTitleEntityId' AND Object_ID = Object_ID(N'Profiles'))
                BEGIN
                    THROW 51000, 'JobTitleEntityId column still exists in Profiles table. Schema validation failed.', 1;
                END

                -- Check for JobTitles table and data
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'JobTitles')
                BEGIN
                    THROW 51000, 'JobTitles table does not exist. Schema validation failed.', 1;
                END

                -- Check for foreign key relationship using correct column references
                IF NOT EXISTS (
                    SELECT 1 
                    FROM sys.foreign_keys fk
                    JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    JOIN sys.tables pt ON fkc.parent_object_id = pt.object_id
                    JOIN sys.columns pc ON fkc.parent_object_id = pc.object_id AND fkc.parent_column_id = pc.column_id
                    WHERE pt.name = 'Profiles' AND pc.name = 'JobTitleId'
                )
                BEGIN
                    THROW 51000, 'Foreign key for JobTitleId in Profiles table does not exist. Schema validation failed.', 1;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op for down migration
        }
    }
}
