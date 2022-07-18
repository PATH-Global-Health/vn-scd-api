using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addempIDtoRefertickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "ReferTickets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "ReferTickets");
        }
    }
}
