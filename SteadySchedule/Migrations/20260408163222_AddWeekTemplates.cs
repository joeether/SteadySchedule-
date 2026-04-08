using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteadySchedule.Migrations
{
    /// <inheritdoc />
    public partial class AddWeekTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeekTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeekTemplateShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeekTemplateId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekTemplateShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekTemplateShifts_WeekTemplates_WeekTemplateId",
                        column: x => x.WeekTemplateId,
                        principalTable: "WeekTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeekTemplateShifts_WeekTemplateId",
                table: "WeekTemplateShifts",
                column: "WeekTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeekTemplateShifts");

            migrationBuilder.DropTable(
                name: "WeekTemplates");
        }
    }
}
