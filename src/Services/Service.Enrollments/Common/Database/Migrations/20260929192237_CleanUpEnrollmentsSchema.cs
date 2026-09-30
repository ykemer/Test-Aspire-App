using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Enrollments.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class CleanUpEnrollmentsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Classes",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentFirstName_StudentLastName",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments");

            migrationBuilder.AlterColumn<string>(
                name: "Operation",
                table: "IdempotencyRecords",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "Which operation this key was recorded for (Enroll or Unenroll)",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 50,
                oldComment: "Which operation this key was recorded for (Enroll or Unenroll)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the enrollment was last updated",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the class was last updated");

            migrationBuilder.AlterColumn<string>(
                name: "StudentLastName",
                table: "Enrollments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "Student's last name",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 100,
                oldComment: "Student's last name");

            migrationBuilder.AlterColumn<string>(
                name: "StudentFirstName",
                table: "Enrollments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "Student's first name",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 100,
                oldComment: "Student's first name");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EnrollmentDateTime",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Date and time when the student enrolled (UTC)",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the enrollment was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the class was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "Classes",
                type: "uuid",
                nullable: false,
                comment: "Course the class belongs to",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldComment: "Foreign key to the course");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Classes",
                type: "uuid",
                nullable: false,
                comment: "Unique identifier (same as in the Courses service)",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldComment: "Unique identifier");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Classes",
                table: "Enrollments",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Classes",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments");

            migrationBuilder.AlterColumn<string>(
                name: "Operation",
                table: "IdempotencyRecords",
                type: "text",
                maxLength: 50,
                nullable: false,
                comment: "Which operation this key was recorded for (Enroll or Unenroll)",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "Which operation this key was recorded for (Enroll or Unenroll)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the class was last updated",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the enrollment was last updated");

            migrationBuilder.AlterColumn<string>(
                name: "StudentLastName",
                table: "Enrollments",
                type: "text",
                maxLength: 100,
                nullable: false,
                comment: "Student's last name",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "Student's last name");

            migrationBuilder.AlterColumn<string>(
                name: "StudentFirstName",
                table: "Enrollments",
                type: "text",
                maxLength: 100,
                nullable: false,
                comment: "Student's first name",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "Student's first name");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EnrollmentDateTime",
                table: "Enrollments",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Date and time when the student enrolled (UTC)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Date and time when the class was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Date and time when the enrollment was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "Classes",
                type: "uuid",
                nullable: false,
                comment: "Foreign key to the course",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldComment: "Course the class belongs to");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Classes",
                type: "uuid",
                nullable: false,
                comment: "Unique identifier",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldComment: "Unique identifier (same as in the Courses service)");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId")
                .Annotation("Npgsql:IndexMethod", "BTREE");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentFirstName_StudentLastName",
                table: "Enrollments",
                columns: new[] { "StudentFirstName", "StudentLastName" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId")
                .Annotation("Npgsql:IndexMethod", "BTREE");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Classes",
                table: "Enrollments",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }
    }
}
