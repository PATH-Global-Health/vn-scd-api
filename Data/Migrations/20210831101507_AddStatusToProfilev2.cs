using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddStatusToProfilev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "People");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Profiles");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "People",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
