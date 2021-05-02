using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartTerraAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Login = table.Column<string>(maxLength: 30, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceJobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExecutionTime = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Done = table.Column<bool>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    DeviceId = table.Column<int>(nullable: false),
                    JobId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceJobs_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceJobs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    DeviceId = table.Column<int>(nullable: false),
                    isLiquidLevelSufficient = table.Column<bool>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    Humidity = table.Column<double>(nullable: false),
                    HeatIndex = table.Column<double>(nullable: false),
                    SoilMoisturePercentage = table.Column<double>(nullable: false),
                    LEDHexColor = table.Column<string>(nullable: false),
                    LEDBrightness = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceProperties_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    IsOn = table.Column<bool>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    Humidity = table.Column<double>(nullable: false),
                    TwilightHour = table.Column<TimeSpan>(nullable: false),
                    HourOfDawn = table.Column<TimeSpan>(nullable: false),
                    DeviceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modes_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Description", "Name", "Type" },
                values: new object[] { 1, "Turn on the LED strip and set color of the LEDs .", "TurnOnLED", "LED" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Description", "Name", "Type" },
                values: new object[] { 2, "Turn off the LED strip.", "TurnOffLED", "LED" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Description", "Name", "Type" },
                values: new object[] { 3, "Turn on the water pump for given period of time.", "TurnOnWaterPump", "PUMP" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Description", "Name", "Type" },
                values: new object[] { 4, "Turn on rainbow.", "Rainbow", "LED" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Description", "Name", "Type" },
                values: new object[] { 5, "Turn right description.", "TurnRight", "ROBOTIC_ARM" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Description", "Name", "Type" },
                values: new object[] { 6, "Turn left description.", "TurnLeft", "ROBOTIC_ARM" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Login", "Password" },
                values: new object[] { 1, "ola@email.com", "ola", "pass1" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Login", "Password" },
                values: new object[] { 2, "robolab@email.com", "robolab", "pass1" });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[] { 1, "SmartTerra v1", 1 });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[] { 101, "ROBOLab test device 1", 2 });

            migrationBuilder.InsertData(
                table: "DeviceJobs",
                columns: new[] { "Id", "Body", "CreatedDate", "DeviceId", "Done", "ExecutionTime", "JobId" },
                values: new object[] { 11, "angle: 10, speed: 2", new DateTime(2021, 4, 27, 22, 44, 50, 994, DateTimeKind.Local).AddTicks(290), 101, false, new DateTime(2021, 4, 27, 22, 44, 50, 975, DateTimeKind.Local).AddTicks(6640), 5 });

            migrationBuilder.InsertData(
                table: "DeviceJobs",
                columns: new[] { "Id", "Body", "CreatedDate", "DeviceId", "Done", "ExecutionTime", "JobId" },
                values: new object[] { 12, "angle: 50, speed: 3", new DateTime(2021, 4, 27, 22, 44, 50, 994, DateTimeKind.Local).AddTicks(2670), 101, false, new DateTime(2021, 4, 27, 22, 44, 50, 994, DateTimeKind.Local).AddTicks(2480), 6 });

            migrationBuilder.InsertData(
                table: "DeviceJobs",
                columns: new[] { "Id", "Body", "CreatedDate", "DeviceId", "Done", "ExecutionTime", "JobId" },
                values: new object[] { 13, "angle: 25, speed: 1", new DateTime(2021, 4, 27, 22, 44, 50, 994, DateTimeKind.Local).AddTicks(2820), 101, false, new DateTime(2021, 4, 27, 22, 44, 50, 994, DateTimeKind.Local).AddTicks(2810), 6 });

            migrationBuilder.InsertData(
                table: "Modes",
                columns: new[] { "Id", "DeviceId", "HourOfDawn", "Humidity", "IsOn", "Name", "Temperature", "TwilightHour" },
                values: new object[] { 1, 1, new TimeSpan(0, 0, 0, 0, 0), 0.0, false, "Terrarium mode 1", 25.0, new TimeSpan(0, 0, 0, 0, 0) });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceJobs_DeviceId",
                table: "DeviceJobs",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceJobs_JobId",
                table: "DeviceJobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceProperties_DeviceId",
                table: "DeviceProperties",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UserId",
                table: "Devices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Modes_DeviceId",
                table: "Modes",
                column: "DeviceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceJobs");

            migrationBuilder.DropTable(
                name: "DeviceProperties");

            migrationBuilder.DropTable(
                name: "Modes");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
