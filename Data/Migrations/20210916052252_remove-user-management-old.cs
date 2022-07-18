using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class removeusermanagementold : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourcePermissionRoles");

            migrationBuilder.DropTable(
                name: "UiPermissionRoles");

            migrationBuilder.DropTable(
                name: "ResourcePermissions");

            migrationBuilder.DropTable(
                name: "UiPermissions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "AspNetRoles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ResourcePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissionType = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UiPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UiPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourcePermissionRoles",
                columns: table => new
                {
                    ResourcePermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePermissionRoles", x => new { x.ResourcePermissionId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ResourcePermissionRoles_ResourcePermissions_ResourcePermissionId",
                        column: x => x.ResourcePermissionId,
                        principalTable: "ResourcePermissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResourcePermissionRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UiPermissionRoles",
                columns: table => new
                {
                    UiPermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UiPermissionRoles", x => new { x.UiPermissionId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UiPermissionRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UiPermissionRoles_UiPermissions_UiPermissionId",
                        column: x => x.UiPermissionId,
                        principalTable: "UiPermissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePermissionRoles_RoleId",
                table: "ResourcePermissionRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UiPermissionRoles_RoleId",
                table: "UiPermissionRoles",
                column: "RoleId");
        }
    }
}
