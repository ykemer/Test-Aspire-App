using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Enrollments.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Enrollments",
          columns: table => new
          {
            Id = table.Column<string>(type: "text", nullable: false),
            EnrollmentDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            CourseId = table.Column<string>(type: "text", nullable: false),
            StudentId = table.Column<string>(type: "text", nullable: false),
            StudentFirstName = table.Column<string>(type: "text", nullable: false),
            StudentLastName = table.Column<string>(type: "text", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Enrollments", x => x.Id);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Enrollments_CourseId",
          table: "Enrollments",
          column: "CourseId");

      migrationBuilder.CreateIndex(
          name: "IX_Enrollments_StudentId",
          table: "Enrollments",
          column: "StudentId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Enrollments");
    }
  }
}
