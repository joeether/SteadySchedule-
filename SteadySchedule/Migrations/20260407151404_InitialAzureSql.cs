using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteadySchedule.Migrations
{
    /// <inheritdoc />
    public partial class InitialAzureSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxHoursPerWeek = table.Column<int>(type: "int", nullable: false),
                    PositionsQualified = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MondayAvailable = table.Column<bool>(type: "bit", nullable: false),
                    MondayStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    MondayEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    TuesdayAvailable = table.Column<bool>(type: "bit", nullable: false),
                    TuesdayStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    TuesdayEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    WednesdayAvailable = table.Column<bool>(type: "bit", nullable: false),
                    WednesdayStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    WednesdayEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    ThursdayAvailable = table.Column<bool>(type: "bit", nullable: false),
                    ThursdayStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    ThursdayEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    FridayAvailable = table.Column<bool>(type: "bit", nullable: false),
                    FridayStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    FridayEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    SaturdayAvailable = table.Column<bool>(type: "bit", nullable: false),
                    SaturdayStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    SaturdayEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    SundayAvailable = table.Column<bool>(type: "bit", nullable: false),
                    SundayStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    SundayEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    MondayAnyTime = table.Column<bool>(type: "bit", nullable: false),
                    TuesdayAnyTime = table.Column<bool>(type: "bit", nullable: false),
                    WednesdayAnyTime = table.Column<bool>(type: "bit", nullable: false),
                    ThursdayAnyTime = table.Column<bool>(type: "bit", nullable: false),
                    FridayAnyTime = table.Column<bool>(type: "bit", nullable: false),
                    SaturdayAnyTime = table.Column<bool>(type: "bit", nullable: false),
                    SundayAnyTime = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    WeekStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SlotGroupId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ApprovedOvertime = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assignments_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_EmployeeId",
                table: "Assignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ShiftId",
                table: "Assignments",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Shifts");
        }
    }
}
