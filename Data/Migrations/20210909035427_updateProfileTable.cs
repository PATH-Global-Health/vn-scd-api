using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class updateProfileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Profiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceptionId",
                table: "Profiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ReceptionId",
                table: "Profiles");
        }
    }
}
