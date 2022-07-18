using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class fixTableReferTickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferTickets_Units_UnitId",
                table: "ReferTickets");

            migrationBuilder.DropIndex(
                name: "IX_ReferTickets_UnitId",
                table: "ReferTickets");

            migrationBuilder.DropColumn(
                name: "Received",
                table: "ReferTickets");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "ReferTickets");

            migrationBuilder.AddColumn<Guid>(
                name: "FromUnitId",
                table: "ReferTickets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "ReferTickets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ToUnitId",
                table: "ReferTickets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferTickets_FromUnitId",
                table: "ReferTickets",
                column: "FromUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferTickets_ToUnitId",
                table: "ReferTickets",
                column: "ToUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferTickets_Units_FromUnitId",
                table: "ReferTickets",
                column: "FromUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferTickets_Units_ToUnitId",
                table: "ReferTickets",
                column: "ToUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferTickets_Units_FromUnitId",
                table: "ReferTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferTickets_Units_ToUnitId",
                table: "ReferTickets");

            migrationBuilder.DropIndex(
                name: "IX_ReferTickets_FromUnitId",
                table: "ReferTickets");

            migrationBuilder.DropIndex(
                name: "IX_ReferTickets_ToUnitId",
                table: "ReferTickets");

            migrationBuilder.DropColumn(
                name: "FromUnitId",
                table: "ReferTickets");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "ReferTickets");

            migrationBuilder.DropColumn(
                name: "ToUnitId",
                table: "ReferTickets");

            migrationBuilder.AddColumn<DateTime>(
                name: "Received",
                table: "ReferTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                table: "ReferTickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferTickets_UnitId",
                table: "ReferTickets",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferTickets_Units_UnitId",
                table: "ReferTickets",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
