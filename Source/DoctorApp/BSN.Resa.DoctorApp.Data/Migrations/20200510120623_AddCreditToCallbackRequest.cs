using Microsoft.EntityFrameworkCore.Migrations;

namespace BSN.Resa.DoctorApp.Data.Migrations
{
    public partial class AddCreditToCallbackRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Credit",
                table: "CallbackRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "CallbackRequests");
        }
    }
}
