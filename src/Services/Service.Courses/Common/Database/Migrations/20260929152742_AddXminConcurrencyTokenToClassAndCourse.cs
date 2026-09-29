using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Courses.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddXminConcurrencyTokenToClassAndCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // xmin is a PostgreSQL system column, present implicitly on every table already — it must
            // NOT be added via DDL (Postgres rejects a column named "xmin" as a name conflict with the
            // system column). The scaffolder generates AddColumn operations for it by mistake; there is
            // no real schema change needed for this migration, only an EF model-snapshot update.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
