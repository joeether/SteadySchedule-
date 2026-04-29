using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteadySchedule.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyBillingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialStartDate",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialEndDate",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow.AddDays(30));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Trialing");

            migrationBuilder.AddColumn<int>(
                name: "BillingAnchorDay",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("IsActive", "Companies");
            migrationBuilder.DropColumn("TrialStartDate", "Companies");
            migrationBuilder.DropColumn("TrialEndDate", "Companies");
            migrationBuilder.DropColumn("Status", "Companies");
            migrationBuilder.DropColumn("BillingAnchorDay", "Companies");
        }
    }
}
