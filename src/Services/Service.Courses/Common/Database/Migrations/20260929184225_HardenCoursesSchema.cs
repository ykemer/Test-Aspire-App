using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Courses.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class HardenCoursesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the course was last updated",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the class was last updated");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "Name of the course",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 255,
                oldComment: "Name of the course");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                comment: "Description of the course",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 2048,
                oldComment: "Description of the course");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the course was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the class was created");

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Id of the handled message"),
                    MessageType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Kind of the handled message"),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Date and time when the message was handled")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => new { x.MessageId, x.MessageType });
                });

            // Two courses cannot have the same name, ignoring upper/lower case.
            // Written by hand because EF Core cannot describe an index on lower("Name").
            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX \"IX_Courses_Name_LowerCase_Unique\" ON \"Courses\" (lower(\"Name\"));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX \"IX_Courses_Name_LowerCase_Unique\";");

            migrationBuilder.DropTable(
                name: "InboxMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the class was last updated",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the course was last updated");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "text",
                maxLength: 255,
                nullable: false,
                comment: "Name of the course",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "Name of the course");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "text",
                maxLength: 2048,
                nullable: false,
                comment: "Description of the course",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldComment: "Description of the course");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the class was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the course was created");
        }
    }
}
