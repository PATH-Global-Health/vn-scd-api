using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SMD_10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientInfos",
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
                    CBOCode = table.Column<string>(maxLength: 16, nullable: true),
                    SupporterName = table.Column<string>(nullable: true),
                    ReachCode = table.Column<string>(maxLength: 16, nullable: true),
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
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientInfos_CBOName_ReachCode",
                table: "PatientInfos",
                columns: new[] { "CBOName", "ReachCode" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientInfos");
        }
    }
}
