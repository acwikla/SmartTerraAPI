using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartTerraAPI.Migrations
{
    public partial class UpdateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceProperties_DeviceId",
                table: "DeviceProperties");

            migrationBuilder.AddColumn<bool>(
                name: "isOn",
                table: "Modes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceProperties_DeviceId",
                table: "DeviceProperties",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceProperties_DeviceId",
                table: "DeviceProperties");

            migrationBuilder.DropColumn(
                name: "isOn",
                table: "Modes");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceProperties_DeviceId",
                table: "DeviceProperties",
                column: "DeviceId",
                unique: true);
        }
    }
}
