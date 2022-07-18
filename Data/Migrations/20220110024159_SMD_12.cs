using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SMD_12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Projects");
        }
    }
}
