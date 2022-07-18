using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SMD_9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "Reports",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Period = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    ValueType = table.Column<int>(nullable: false),
                    CreatedMethod = table.Column<int>(nullable: false),
                    PackageCode = table.Column<string>(nullable: true),
                    TargetValue = table.Column<double>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    CreateBy = table.Column<string>(nullable: true),
                    ReportId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportHistories_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportHistories_ReportId",
                table: "ReportHistories",
                column: "ReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportHistories");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Reports");
        }
    }
}
