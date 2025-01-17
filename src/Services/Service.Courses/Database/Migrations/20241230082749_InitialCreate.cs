using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Courses.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Courses",
          columns: table => new
          {
            Id = table.Column<string>(type: "text", nullable: false),
            Name = table.Column<string>(type: "text", nullable: false),
            Description = table.Column<string>(type: "text", nullable: false),
            EnrollmentsCount = table.Column<int>(type: "integer", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Courses", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Courses");
    }
  }
}
