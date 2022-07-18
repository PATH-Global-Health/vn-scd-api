using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class LayTestAndPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "People",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<double>(nullable: false),
                    FacilityId = table.Column<string>(nullable: true),
                    ReceptionId = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    ExternalId = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelatedPatients",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    RelatedId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedPatients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LayTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    FacilityId = table.Column<string>(nullable: true),
                    HIVPublicExaminationDate = table.Column<double>(nullable: false),
                    PublicExaminationOrder = table.Column<string>(nullable: true),
                    PublicExaminationCode = table.Column<string>(nullable: true),
                    ExaminationForm = table.Column<int>(nullable: false),
                    ReceptionId = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    PatientId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LayTests_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LayTests_PatientId",
                table: "LayTests",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LayTests");

            migrationBuilder.DropTable(
                name: "RelatedPatients");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "People");
        }
    }
}
