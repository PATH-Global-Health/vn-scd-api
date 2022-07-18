using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SMD_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Packages_PackageId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_PackageId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Units");

            migrationBuilder.CreateTable(
                name: "ImplementPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Province = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<double>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImplementPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImplementPackages_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: true),
                    IsCurrent = table.Column<bool>(nullable: false),
                    CBOId = table.Column<Guid>(nullable: false),
                    IPackageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Units_CBOId",
                        column: x => x.CBOId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_ImplementPackages_IPackageId",
                        column: x => x.IPackageId,
                        principalTable: "ImplementPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Targets",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    IPackageId = table.Column<Guid>(nullable: false),
                    IndicatorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Targets_ImplementPackages_IPackageId",
                        column: x => x.IPackageId,
                        principalTable: "ImplementPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Targets_Indicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalTable: "Indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CBOId",
                table: "Contracts",
                column: "CBOId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_IPackageId",
                table: "Contracts",
                column: "IPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImplementPackages_PackageId",
                table: "ImplementPackages",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Targets_IPackageId",
                table: "Targets",
                column: "IPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Targets_IndicatorId",
                table: "Targets",
                column: "IndicatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Targets");

            migrationBuilder.DropTable(
                name: "ImplementPackages");

            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "Units",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_PackageId",
                table: "Units",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Packages_PackageId",
                table: "Units",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
