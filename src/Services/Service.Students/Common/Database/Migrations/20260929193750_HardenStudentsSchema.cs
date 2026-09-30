using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Students.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class HardenStudentsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_FirstName_LastName_Email",
                table: "Students");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the student was last updated",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the class was last updated");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentsCount",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Number of classes the student is enrolled in",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0,
                oldComment: "Number of enrollments the student has");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the student was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the class was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Students",
                type: "uuid",
                nullable: false,
                comment: "Unique identifier (same as the user id in Platform)",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldComment: "Unique identifier");

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

            // Two students cannot share an email, ignoring upper/lower case.
            // Written by hand because EF Core cannot describe an index on lower("Email").
            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX \"IX_Students_Email_LowerCase_Unique\" ON \"Students\" (lower(\"Email\"));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX \"IX_Students_Email_LowerCase_Unique\";");

            migrationBuilder.DropTable(
                name: "InboxMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the class was last updated",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the student was last updated");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentsCount",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Number of enrollments the student has",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0,
                oldComment: "Number of classes the student is enrolled in");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the class was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the student was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Students",
                type: "uuid",
                nullable: false,
                comment: "Unique identifier",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldComment: "Unique identifier (same as the user id in Platform)");

            migrationBuilder.CreateIndex(
                name: "IX_Students_FirstName_LastName_Email",
                table: "Students",
                columns: new[] { "FirstName", "LastName", "Email" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }
    }
}
