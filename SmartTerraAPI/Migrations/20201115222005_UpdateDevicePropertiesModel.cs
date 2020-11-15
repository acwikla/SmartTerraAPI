using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartTerraAPI.Migrations
{
    public partial class UpdateDevicePropertiesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeatIndex",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "isWaterLevelSufficient",
                table: "DeviceProperties");

            migrationBuilder.AddColumn<double>(
                name: "SoilMoisturePercentage",
                table: "DeviceProperties",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isLiquidLevelSufficient",
                table: "DeviceProperties",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoilMoisturePercentage",
                table: "DeviceProperties");

            migrationBuilder.DropColumn(
                name: "isLiquidLevelSufficient",
                table: "DeviceProperties");

            migrationBuilder.AddColumn<double>(
                name: "HeatIndex",
                table: "Modes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isWaterLevelSufficient",
                table: "DeviceProperties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
