using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartTerraAPI.Migrations
{
    public partial class ChangeIsOnFlagName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isOn",
                table: "Modes",
                newName: "IsOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOn",
                table: "Modes",
                newName: "isOn");
        }
    }
}
