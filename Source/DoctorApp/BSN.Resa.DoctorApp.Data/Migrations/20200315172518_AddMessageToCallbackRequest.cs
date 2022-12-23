using Microsoft.EntityFrameworkCore.Migrations;

namespace BSN.Resa.DoctorApp.Data.Migrations
{
    public partial class AddMessageToCallbackRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "CallbackRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "CallbackRequests");
        }
    }
}
