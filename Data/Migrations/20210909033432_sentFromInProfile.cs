using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class sentFromInProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SentFrom",
                table: "Profiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentFrom",
                table: "Profiles");
        }
    }
}
