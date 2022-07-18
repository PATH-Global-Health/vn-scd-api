using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SMD_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "Units",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Units",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IndicatorId",
                table: "Reports",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                table: "Reports",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IndicatorId",
                table: "KPIs",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Units_PackageId",
                table: "Units",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_ProjectId",
                table: "Units",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_IndicatorId",
                table: "Reports",
                column: "IndicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UnitId",
                table: "Reports",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_IndicatorId",
                table: "KPIs",
                column: "IndicatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_KPIs_Indicators_IndicatorId",
                table: "KPIs",
                column: "IndicatorId",
                principalTable: "Indicators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Indicators_IndicatorId",
                table: "Reports",
                column: "IndicatorId",
                principalTable: "Indicators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Units_UnitId",
                table: "Reports",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Packages_PackageId",
                table: "Units",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Projects_ProjectId",
                table: "Units",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIs_Indicators_IndicatorId",
                table: "KPIs");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Indicators_IndicatorId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Units_UnitId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_Packages_PackageId",
                table: "Units");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_Projects_ProjectId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_PackageId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_ProjectId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Reports_IndicatorId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_UnitId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_KPIs_IndicatorId",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "IndicatorId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IndicatorId",
                table: "KPIs");
        }
    }
}
