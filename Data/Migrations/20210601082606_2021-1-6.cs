using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _202116 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InjectionObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    FromDaysOld = table.Column<int>(nullable: true),
                    ToDaysOld = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InjectionObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Code = table.Column<string>(nullable: true),
                    PersonRole = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    IdentityCard = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    AcademicTitle = table.Column<string>(nullable: true),
                    Gender = table.Column<bool>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Fullname = table.Column<string>(nullable: true),
                    Gender = table.Column<bool>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    VaccinationCode = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    District = table.Column<string>(nullable: true),
                    Ward = table.Column<string>(nullable: true),
                    IdentityCard = table.Column<string>(nullable: true),
                    PassportNumber = table.Column<string>(nullable: true),
                    Nation = table.Column<string>(nullable: true),
                    RelationProfileId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Profiles_RelationProfileId",
                        column: x => x.RelationProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    CanChooseDoctor = table.Column<bool>(nullable: false),
                    CanUseHealthInsurance = table.Column<bool>(nullable: false),
                    CanChooseHour = table.Column<bool>(nullable: false),
                    CanPostPay = table.Column<bool>(nullable: false),
                    UnitId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Code = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInformation",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    District = table.Column<string>(nullable: true),
                    Ward = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Introduction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInformation", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "WorkingCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UnitId = table.Column<Guid>(nullable: false),
                    BookingBeforeDate = table.Column<int>(nullable: false),
                    BookingAfterDate = table.Column<int>(nullable: false),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ToDate = table.Column<DateTime>(nullable: false),
                    ShiftCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitDoctors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    DoctorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitDoctors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitDoctors_People_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InjectionObjectServiceTypes",
                columns: table => new
                {
                    InjectionObjectId = table.Column<Guid>(nullable: false),
                    ServiceTypeId = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InjectionObjectServiceTypes", x => new { x.ServiceTypeId, x.InjectionObjectId });
                    table.ForeignKey(
                        name: "FK_InjectionObjectServiceTypes_InjectionObjects_InjectionObjectId",
                        column: x => x.InjectionObjectId,
                        principalTable: "InjectionObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InjectionObjectServiceTypes_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    JobType = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CommonResource = table.Column<bool>(nullable: true),
                    Price = table.Column<double>(nullable: true),
                    ServiceFormId = table.Column<Guid>(nullable: true),
                    ServiceTypeId = table.Column<Guid>(nullable: true),
                    InjectionObjectId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_InjectionObjects_InjectionObjectId",
                        column: x => x.InjectionObjectId,
                        principalTable: "InjectionObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_ServiceForms_ServiceFormId",
                        column: x => x.ServiceFormId,
                        principalTable: "ServiceForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jobs_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    District = table.Column<string>(nullable: true),
                    Ward = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Introduction = table.Column<string>(nullable: true),
                    Logo = table.Column<byte[]>(nullable: true),
                    UnitTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_UnitTypes_UnitTypeId",
                        column: x => x.UnitTypeId,
                        principalTable: "UnitTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    CalendarId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Days_WorkingCalendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "WorkingCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorWorkingCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WorkingCalendarId = table.Column<Guid>(nullable: false),
                    DoctorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorWorkingCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorWorkingCalendars_People_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorWorkingCalendars_WorkingCalendars_WorkingCalendarId",
                        column: x => x.WorkingCalendarId,
                        principalTable: "WorkingCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkingCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WorkingCalendarId = table.Column<Guid>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceWorkingCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceWorkingCalendars_Jobs_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceWorkingCalendars_WorkingCalendars_WorkingCalendarId",
                        column: x => x.WorkingCalendarId,
                        principalTable: "WorkingCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    PlaceType = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    UnitId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Places_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false),
                    UnitId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceUnits_Jobs_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceUnits_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Image = table.Column<byte[]>(nullable: true),
                    UnitId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitImages_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    From = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    DayId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomWorkingCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WorkingCalendarId = table.Column<Guid>(nullable: false),
                    RoomId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomWorkingCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomWorkingCalendars_Places_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomWorkingCalendars_WorkingCalendars_WorkingCalendarId",
                        column: x => x.WorkingCalendarId,
                        principalTable: "WorkingCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Intervals",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    From = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    IsAvailable = table.Column<bool>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    NumId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intervals_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleJobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleJobs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleJobs_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulePeople",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulePeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulePeople_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchedulePeople_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulePlaces",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaceId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulePlaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulePlaces_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchedulePlaces_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Days_CalendarId",
                table: "Days",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorWorkingCalendars_DoctorId",
                table: "DoctorWorkingCalendars",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorWorkingCalendars_WorkingCalendarId",
                table: "DoctorWorkingCalendars",
                column: "WorkingCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_InjectionObjectServiceTypes_InjectionObjectId",
                table: "InjectionObjectServiceTypes",
                column: "InjectionObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Intervals_ScheduleId",
                table: "Intervals",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_InjectionObjectId",
                table: "Jobs",
                column: "InjectionObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ServiceFormId",
                table: "Jobs",
                column: "ServiceFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ServiceTypeId",
                table: "Jobs",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Places_UnitId",
                table: "Places",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_RelationProfileId",
                table: "Profiles",
                column: "RelationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomWorkingCalendars_RoomId",
                table: "RoomWorkingCalendars",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomWorkingCalendars_WorkingCalendarId",
                table: "RoomWorkingCalendars",
                column: "WorkingCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleJobs_JobId",
                table: "ScheduleJobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleJobs_ScheduleId",
                table: "ScheduleJobs",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulePeople_PersonId",
                table: "SchedulePeople",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulePeople_ScheduleId",
                table: "SchedulePeople",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulePlaces_PlaceId",
                table: "SchedulePlaces",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulePlaces_ScheduleId",
                table: "SchedulePlaces",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DayId",
                table: "Schedules",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceUnits_Code",
                table: "ServiceUnits",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceUnits_ServiceId",
                table: "ServiceUnits",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceUnits_UnitId",
                table: "ServiceUnits",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkingCalendars_ServiceId",
                table: "ServiceWorkingCalendars",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkingCalendars_WorkingCalendarId",
                table: "ServiceWorkingCalendars",
                column: "WorkingCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitDoctors_DoctorId",
                table: "UnitDoctors",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitImages_UnitId",
                table: "UnitImages",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitTypeId",
                table: "Units",
                column: "UnitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTypes_Code",
                table: "UnitTypes",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DoctorWorkingCalendars");

            migrationBuilder.DropTable(
                name: "InjectionObjectServiceTypes");

            migrationBuilder.DropTable(
                name: "Intervals");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "RoomWorkingCalendars");

            migrationBuilder.DropTable(
                name: "ScheduleJobs");

            migrationBuilder.DropTable(
                name: "SchedulePeople");

            migrationBuilder.DropTable(
                name: "SchedulePlaces");

            migrationBuilder.DropTable(
                name: "ServiceUnits");

            migrationBuilder.DropTable(
                name: "ServiceWorkingCalendars");

            migrationBuilder.DropTable(
                name: "UnitDoctors");

            migrationBuilder.DropTable(
                name: "UnitImages");

            migrationBuilder.DropTable(
                name: "UserInformation");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.DropTable(
                name: "InjectionObjects");

            migrationBuilder.DropTable(
                name: "ServiceForms");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "UnitTypes");

            migrationBuilder.DropTable(
                name: "WorkingCalendars");
        }
    }
}
