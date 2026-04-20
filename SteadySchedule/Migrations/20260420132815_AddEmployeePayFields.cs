using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteadySchedule.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSalary",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "WeeklySalary",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsSalary",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "WeeklySalary",
                table: "Employees");
        }
    }
}
