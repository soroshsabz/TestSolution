using Microsoft.EntityFrameworkCore.Migrations;

namespace BSN.Resa.DoctorApp.Data.Migrations
{
    public partial class AddIsCancelledToCallbackRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "CallbackRequests",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "CallbackRequests");
        }
    }
}
