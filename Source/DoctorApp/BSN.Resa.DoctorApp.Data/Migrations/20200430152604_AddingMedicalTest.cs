using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BSN.Resa.DoctorApp.Data.Migrations
{
    public partial class AddingMedicalTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalTests",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    Photos = table.Column<string>(nullable: true),
                    PatientId = table.Column<string>(nullable: true),
                    PatientPhone = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalTests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalTests");
        }
    }
}
