using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Data.Migrations
{
    public partial class UpdateCallbackRequestsTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEstablishedCallNotified",
                table: "CallbackRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCallTriedAt",
                table: "CallbackRequests",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEstablishedCallNotified",
                table: "CallbackRequests");

            migrationBuilder.DropColumn(
                name: "LastCallTriedAt",
                table: "CallbackRequests");
        }
    }
}
