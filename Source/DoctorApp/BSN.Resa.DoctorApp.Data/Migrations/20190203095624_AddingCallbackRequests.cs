using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Data.Migrations
{
    public partial class AddingCallbackRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CallbackRequests",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CallerFullName = table.Column<string>(nullable: true),
                    CallerSubscriberNumber = table.Column<string>(nullable: true),
                    CommunicationAttemptsCount = table.Column<int>(nullable: false),
                    ConsentGivenAt = table.Column<DateTime>(nullable: false),
                    IsCallTried = table.Column<bool>(nullable: false),
                    IsSeen = table.Column<bool>(nullable: false),
                    ReceiverFullName = table.Column<string>(nullable: true),
                    ReceiverSubscriberNumber = table.Column<string>(nullable: true),
                    ReturnCallHasBeenEstablished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallbackRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallbackRequests");
        }
    }
}
