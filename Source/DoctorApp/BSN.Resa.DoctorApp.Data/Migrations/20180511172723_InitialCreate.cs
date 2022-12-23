using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUpdates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HasNotifiableUpdateLocally = table.Column<bool>(nullable: false),
                    HasUrgentUpdateLocally = table.Column<bool>(nullable: false),
                    LastSynchronizationTime = table.Column<DateTime>(nullable: true),
                    LatestDownloadableAppUpdateUrlLocally = table.Column<string>(nullable: true),
                    LatestDownloadableAppUpdateVersionLocallyInString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUpdates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(nullable: true),
                    IsLoggedIn = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    Msisdn = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    ServiceCommuncationToken_AccessToken = table.Column<string>(nullable: true),
                    ServiceCommuncationToken_ExpiresIn = table.Column<string>(nullable: true),
                    ServiceCommuncationToken_TokenType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BlockedCount = table.Column<int>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false),
                    IsAnnouncedToService = table.Column<bool>(nullable: false),
                    IsBlocked = table.Column<bool>(nullable: false),
                    IsResaContact = table.Column<bool>(nullable: false),
                    IsVisible = table.Column<bool>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_DoctorId",
                table: "Contacts",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_PhoneNumber",
                table: "Contacts",
                column: "PhoneNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUpdates");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Doctors");
        }
    }
}
