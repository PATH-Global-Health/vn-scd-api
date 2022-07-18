using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addIsTesting_PrEp_ART : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsARTFacility",
                table: "Units",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrEPFacility",
                table: "Units",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTestingFacility",
                table: "Units",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsARTFacility",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "IsPrEPFacility",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "IsTestingFacility",
                table: "Units");
        }
    }
}
