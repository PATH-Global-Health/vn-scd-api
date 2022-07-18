using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addParentIdToUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Units",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_ParentId",
                table: "Units",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Units_ParentId",
                table: "Units",
                column: "ParentId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Units_ParentId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_ParentId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Units");
        }
    }
}
