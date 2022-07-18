using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SMD_4_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DenominatorIndicatorId",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NumeratorIndicatorId",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Indicators",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Indicators_DenominatorIndicatorId",
                table: "Indicators",
                column: "DenominatorIndicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Indicators_NumeratorIndicatorId",
                table: "Indicators",
                column: "NumeratorIndicatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Indicators_Indicators_DenominatorIndicatorId",
                table: "Indicators",
                column: "DenominatorIndicatorId",
                principalTable: "Indicators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Indicators_Indicators_NumeratorIndicatorId",
                table: "Indicators",
                column: "NumeratorIndicatorId",
                principalTable: "Indicators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indicators_Indicators_DenominatorIndicatorId",
                table: "Indicators");

            migrationBuilder.DropForeignKey(
                name: "FK_Indicators_Indicators_NumeratorIndicatorId",
                table: "Indicators");

            migrationBuilder.DropIndex(
                name: "IX_Indicators_DenominatorIndicatorId",
                table: "Indicators");

            migrationBuilder.DropIndex(
                name: "IX_Indicators_NumeratorIndicatorId",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "DenominatorIndicatorId",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "NumeratorIndicatorId",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Indicators");
        }
    }
}
