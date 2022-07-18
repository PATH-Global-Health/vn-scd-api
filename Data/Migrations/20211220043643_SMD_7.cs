using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SMD_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BlockChanges",
                table: "Units",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockChanges",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockChanges",
                table: "Packages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockChanges",
                table: "Indicators",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockChanges",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "BlockChanges",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BlockChanges",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "BlockChanges",
                table: "Indicators");
        }
    }
}
