using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartGarage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParkingRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehicleType = table.Column<int>(type: "INTEGER", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingSpots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpotCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Zone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Floor = table.Column<int>(type: "INTEGER", nullable: false),
                    SpotType = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSpots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LicensePlate = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    VehicleType = table.Column<int>(type: "INTEGER", nullable: false),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParkingTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TicketCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    QrCode = table.Column<string>(type: "TEXT", nullable: false),
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParkingSpotId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CheckOutTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingTickets_ParkingSpots_ParkingSpotId",
                        column: x => x.ParkingSpotId,
                        principalTable: "ParkingSpots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParkingTickets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParkingTickets_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ParkingTicketId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_ParkingTickets_ParkingTicketId",
                        column: x => x.ParkingTicketId,
                        principalTable: "ParkingTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ParkingRates",
                columns: new[] { "Id", "CreatedAt", "DailyRate", "EffectiveFrom", "EffectiveTo", "HourlyRate", "IsActive", "MonthlyRate", "VehicleType" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 6, 15, 17, 59, 567, DateTimeKind.Utc).AddTicks(8627), 30000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 5000m, true, 500000m, 0 },
                    { 2, new DateTime(2026, 5, 6, 15, 17, 59, 567, DateTimeKind.Utc).AddTicks(8640), 100000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 20000m, true, 2000000m, 1 },
                    { 3, new DateTime(2026, 5, 6, 15, 17, 59, 567, DateTimeKind.Utc).AddTicks(8643), 10000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2000m, true, 200000m, 2 },
                    { 4, new DateTime(2026, 5, 6, 15, 17, 59, 567, DateTimeKind.Utc).AddTicks(8645), 150000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 30000m, true, 3000000m, 3 }
                });

            migrationBuilder.InsertData(
                table: "ParkingSpots",
                columns: new[] { "Id", "CreatedAt", "Floor", "SpotCode", "SpotType", "Status", "UpdatedAt", "Zone" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A101", 1, 0, null, "A" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A102", 1, 0, null, "A" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A103", 1, 0, null, "A" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A104", 1, 0, null, "A" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A105", 1, 0, null, "A" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A106", 1, 0, null, "A" },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A107", 1, 0, null, "A" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A108", 1, 0, null, "A" },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A109", 1, 0, null, "A" },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "A110", 1, 0, null, "A" },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B101", 0, 0, null, "B" },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B102", 0, 0, null, "B" },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B103", 0, 0, null, "B" },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B104", 0, 0, null, "B" },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B105", 0, 0, null, "B" },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B106", 0, 0, null, "B" },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B107", 0, 0, null, "B" },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B108", 0, 0, null, "B" },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B109", 0, 0, null, "B" },
                    { 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B110", 0, 0, null, "B" },
                    { 21, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B111", 0, 0, null, "B" },
                    { 22, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B112", 0, 0, null, "B" },
                    { 23, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B113", 0, 0, null, "B" },
                    { 24, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B114", 0, 0, null, "B" },
                    { 25, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B115", 0, 0, null, "B" },
                    { 26, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B116", 0, 0, null, "B" },
                    { 27, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B117", 0, 0, null, "B" },
                    { 28, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B118", 0, 0, null, "B" },
                    { 29, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B119", 0, 0, null, "B" },
                    { 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "B120", 0, 0, null, "B" },
                    { 31, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A201", 1, 0, null, "A" },
                    { 32, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A202", 1, 0, null, "A" },
                    { 33, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A203", 1, 0, null, "A" },
                    { 34, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A204", 1, 0, null, "A" },
                    { 35, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A205", 1, 0, null, "A" },
                    { 36, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A206", 1, 0, null, "A" },
                    { 37, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A207", 1, 0, null, "A" },
                    { 38, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A208", 1, 0, null, "A" },
                    { 39, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A209", 1, 0, null, "A" },
                    { 40, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "A210", 1, 0, null, "A" },
                    { 41, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B201", 0, 0, null, "B" },
                    { 42, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B202", 0, 0, null, "B" },
                    { 43, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B203", 0, 0, null, "B" },
                    { 44, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B204", 0, 0, null, "B" },
                    { 45, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B205", 0, 0, null, "B" },
                    { 46, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B206", 0, 0, null, "B" },
                    { 47, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B207", 0, 0, null, "B" },
                    { 48, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B208", 0, 0, null, "B" },
                    { 49, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B209", 0, 0, null, "B" },
                    { 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B210", 0, 0, null, "B" },
                    { 51, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B211", 0, 0, null, "B" },
                    { 52, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B212", 0, 0, null, "B" },
                    { 53, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B213", 0, 0, null, "B" },
                    { 54, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B214", 0, 0, null, "B" },
                    { 55, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B215", 0, 0, null, "B" },
                    { 56, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B216", 0, 0, null, "B" },
                    { 57, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B217", 0, 0, null, "B" },
                    { 58, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B218", 0, 0, null, "B" },
                    { 59, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B219", 0, 0, null, "B" },
                    { 60, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "B220", 0, 0, null, "B" },
                    { 61, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A301", 1, 0, null, "A" },
                    { 62, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A302", 1, 0, null, "A" },
                    { 63, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A303", 1, 0, null, "A" },
                    { 64, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A304", 1, 0, null, "A" },
                    { 65, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A305", 1, 0, null, "A" },
                    { 66, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A306", 1, 0, null, "A" },
                    { 67, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A307", 1, 0, null, "A" },
                    { 68, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A308", 1, 0, null, "A" },
                    { 69, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A309", 1, 0, null, "A" },
                    { 70, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "A310", 1, 0, null, "A" },
                    { 71, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B301", 0, 0, null, "B" },
                    { 72, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B302", 0, 0, null, "B" },
                    { 73, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B303", 0, 0, null, "B" },
                    { 74, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B304", 0, 0, null, "B" },
                    { 75, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B305", 0, 0, null, "B" },
                    { 76, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B306", 0, 0, null, "B" },
                    { 77, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B307", 0, 0, null, "B" },
                    { 78, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B308", 0, 0, null, "B" },
                    { 79, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B309", 0, 0, null, "B" },
                    { 80, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B310", 0, 0, null, "B" },
                    { 81, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B311", 0, 0, null, "B" },
                    { 82, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B312", 0, 0, null, "B" },
                    { 83, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B313", 0, 0, null, "B" },
                    { 84, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B314", 0, 0, null, "B" },
                    { 85, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B315", 0, 0, null, "B" },
                    { 86, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B316", 0, 0, null, "B" },
                    { 87, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B317", 0, 0, null, "B" },
                    { 88, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B318", 0, 0, null, "B" },
                    { 89, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B319", 0, 0, null, "B" },
                    { 90, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "B320", 0, 0, null, "B" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Phone", "Role", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@smartgarage.com", "Admin", true, "$2a$11$/LAhUzI2r0NPKa3UO68E3edzT22n6AFwJkrJGUUPZ4fzhJTsvZRTy", "0901234567", 3, null });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpots_SpotCode",
                table: "ParkingSpots",
                column: "SpotCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingTickets_ParkingSpotId",
                table: "ParkingTickets",
                column: "ParkingSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingTickets_TicketCode",
                table: "ParkingTickets",
                column: "TicketCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingTickets_UserId",
                table: "ParkingTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingTickets_VehicleId",
                table: "ParkingTickets",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ParkingTicketId",
                table: "Payments",
                column: "ParkingTicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionCode",
                table: "Payments",
                column: "TransactionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicensePlate",
                table: "Vehicles",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_OwnerId",
                table: "Vehicles",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ParkingRates");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ParkingTickets");

            migrationBuilder.DropTable(
                name: "ParkingSpots");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
