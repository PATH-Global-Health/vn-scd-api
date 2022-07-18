using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SMD_13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "PatientInfos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedMethod",
                table: "PatientInfos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PatientInfoHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    PSNU = table.Column<string>(nullable: true),
                    MoPName = table.Column<string>(nullable: true),
                    CBOName = table.Column<string>(nullable: true),
                    CBOCode = table.Column<string>(nullable: true),
                    CBOId = table.Column<Guid>(nullable: false),
                    SupporterName = table.Column<string>(nullable: true),
                    ReachCode = table.Column<string>(nullable: true),
                    LayTestingCode = table.Column<string>(nullable: true),
                    HTCTestCode = table.Column<string>(nullable: true),
                    HTCSite = table.Column<string>(nullable: true),
                    TestResult = table.Column<string>(nullable: true),
                    DateOfTesting = table.Column<DateTime>(nullable: true),
                    ServiceName = table.Column<string>(nullable: true),
                    ClientID = table.Column<string>(nullable: true),
                    FacilityName = table.Column<string>(nullable: true),
                    DateOfReferral = table.Column<DateTime>(nullable: true),
                    ReferralSlip = table.Column<string>(nullable: true),
                    NewCase = table.Column<string>(nullable: true),
                    DateOfVerification = table.Column<DateTime>(nullable: true),
                    ReportingPeriod = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    CreateBy = table.Column<string>(nullable: true),
                    CreatedMethod = table.Column<int>(nullable: false),
                    PatientInfoId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientInfoHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientInfoHistories_PatientInfos_PatientInfoId",
                        column: x => x.PatientInfoId,
                        principalTable: "PatientInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientInfoHistories_PatientInfoId",
                table: "PatientInfoHistories",
                column: "PatientInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientInfoHistories");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "PatientInfos");

            migrationBuilder.DropColumn(
                name: "CreatedMethod",
                table: "PatientInfos");
        }
    }
}
