using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Airline = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartureAirport = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArrivalAirport = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "USD")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MobileOS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "USD"),
                    IsFlexibleDate = table.Column<bool>(type: "bit", nullable: false),
                    FlexibleDays = table.Column<int>(type: "int", nullable: true),
                    OneWay = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceAlerts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFlights",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlightId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFlights", x => new { x.UserId, x.FlightId });
                    table.ForeignKey(
                        name: "FK_UserFlights_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFlights_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightPriceAlert",
                columns: table => new
                {
                    MatchedAlertsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchedFlightsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPriceAlert", x => new { x.MatchedAlertsId, x.MatchedFlightsId });
                    table.ForeignKey(
                        name: "FK_FlightPriceAlert_Flights_MatchedFlightsId",
                        column: x => x.MatchedFlightsId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightPriceAlert_PriceAlerts_MatchedAlertsId",
                        column: x => x.MatchedAlertsId,
                        principalTable: "PriceAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightPriceAlert_MatchedFlightsId",
                table: "FlightPriceAlert",
                column: "MatchedFlightsId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ArrivalAirport",
                table: "Flights",
                column: "ArrivalAirport");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DepartureTime",
                table: "Flights",
                column: "DepartureTime");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_FlightNumber",
                table: "Flights",
                column: "FlightNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlerts_Destination",
                table: "PriceAlerts",
                column: "Destination");

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlerts_IsActive",
                table: "PriceAlerts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlerts_Origin",
                table: "PriceAlerts",
                column: "Origin");

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlerts_TargetPrice",
                table: "PriceAlerts",
                column: "TargetPrice");

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlerts_UserId",
                table: "PriceAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFlights_FlightId",
                table: "UserFlights",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFlights_UserId",
                table: "UserFlights",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightPriceAlert");

            migrationBuilder.DropTable(
                name: "UserFlights");

            migrationBuilder.DropTable(
                name: "PriceAlerts");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
