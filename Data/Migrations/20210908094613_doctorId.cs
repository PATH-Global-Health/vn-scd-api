using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class doctorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                table: "UnitDoctors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitDoctors_UnitId",
                table: "UnitDoctors",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitDoctors_Units_UnitId",
                table: "UnitDoctors",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitDoctors_Units_UnitId",
                table: "UnitDoctors");

            migrationBuilder.DropIndex(
                name: "IX_UnitDoctors_UnitId",
                table: "UnitDoctors");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "UnitDoctors");
        }
    }
}
