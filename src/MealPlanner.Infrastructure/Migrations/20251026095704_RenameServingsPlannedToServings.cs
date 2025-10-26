using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameServingsPlannedToServings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServingsPlanned",
                table: "PlannedMeals",
                newName: "Servings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Servings",
                table: "PlannedMeals",
                newName: "ServingsPlanned");
        }
    }
}
