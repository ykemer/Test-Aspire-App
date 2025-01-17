using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Students.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Students",
          columns: table => new
          {
            Id = table.Column<string>(type: "text", nullable: false),
            FirstName = table.Column<string>(type: "text", nullable: false),
            LastName = table.Column<string>(type: "text", nullable: false),
            Email = table.Column<string>(type: "text", nullable: false),
            DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            EnrolledCourses = table.Column<int>(type: "integer", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Students", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Students");
    }
  }
}
