using Microsoft.EntityFrameworkCore.Migrations;

namespace ActivityCenter.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationLen",
                table: "DojoActivities");

            migrationBuilder.DropColumn(
                name: "TimeStart",
                table: "DojoActivities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DurationLen",
                table: "DojoActivities",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeStart",
                table: "DojoActivities",
                nullable: false,
                defaultValue: "");
        }
    }
}
