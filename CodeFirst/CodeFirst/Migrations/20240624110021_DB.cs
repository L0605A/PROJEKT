using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class DB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    IdClient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.IdClient);
                });

            migrationBuilder.CreateTable(
                name: "discounts",
                columns: table => new
                {
                    IdDiscount = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Offer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amt = table.Column<int>(type: "int", nullable: false),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discounts", x => x.IdDiscount);
                });

            migrationBuilder.CreateTable(
                name: "softwares",
                columns: table => new
                {
                    IdSoftware = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_softwares", x => x.IdSoftware);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "corporate_clients",
                columns: table => new
                {
                    IdClient = table.Column<int>(type: "int", nullable: false),
                    CorpoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KRS = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_corporate_clients", x => x.IdClient);
                    table.ForeignKey(
                        name: "FK_corporate_clients_clients_IdClient",
                        column: x => x.IdClient,
                        principalTable: "clients",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "personal_clients",
                columns: table => new
                {
                    IdClient = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PESEL = table.Column<decimal>(type: "decimal(11,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personal_clients", x => x.IdClient);
                    table.ForeignKey(
                        name: "FK_personal_clients_clients_IdClient",
                        column: x => x.IdClient,
                        principalTable: "clients",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    IdContract = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClient = table.Column<int>(type: "int", nullable: false),
                    IdSoftware = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.IdContract);
                    table.ForeignKey(
                        name: "FK_contracts_clients_IdClient",
                        column: x => x.IdClient,
                        principalTable: "clients",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contracts_softwares_IdSoftware",
                        column: x => x.IdSoftware,
                        principalTable: "softwares",
                        principalColumn: "IdSoftware",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ledgers",
                columns: table => new
                {
                    IdPayment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdContract = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidOn = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledgers", x => x.IdPayment);
                    table.ForeignKey(
                        name: "FK_ledgers_contracts_IdContract",
                        column: x => x.IdContract,
                        principalTable: "contracts",
                        principalColumn: "IdContract",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "one_time_payments",
                columns: table => new
                {
                    IdContract = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatePeriod = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_one_time_payments", x => x.IdContract);
                    table.ForeignKey(
                        name: "FK_one_time_payments_contracts_IdContract",
                        column: x => x.IdContract,
                        principalTable: "contracts",
                        principalColumn: "IdContract",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    IdContract = table.Column<int>(type: "int", nullable: false),
                    RenevalTimeInMonths = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.IdContract);
                    table.ForeignKey(
                        name: "FK_subscriptions_contracts_IdContract",
                        column: x => x.IdContract,
                        principalTable: "contracts",
                        principalColumn: "IdContract",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "clients",
                columns: new[] { "IdClient", "Address", "Email", "IsDeleted", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "123 Main St", "client1@example.com", false, "1234567890" },
                    { 2, "456 Elm St", "client2@example.com", false, "9876543210" }
                });

            migrationBuilder.InsertData(
                table: "discounts",
                columns: new[] { "IdDiscount", "Amt", "DateFrom", "DateTo", "Name", "Offer" },
                values: new object[] { 1, 10, new DateOnly(2024, 6, 24), new DateOnly(2024, 7, 24), "Discount1", "Offer1" });

            migrationBuilder.InsertData(
                table: "softwares",
                columns: new[] { "IdSoftware", "Description", "Name", "Type", "Version" },
                values: new object[] { 1, "Description1", "Software1", "Type1", "1.0" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", "Admin", "admin" },
                    { 2, "e606e38b0d8c19b24cf0ee3808183162ea7cd63ff7912dbb22b5e803286b4446", "User", "user" }
                });

            migrationBuilder.InsertData(
                table: "contracts",
                columns: new[] { "IdContract", "DateFrom", "IdClient", "IdSoftware", "Name", "Price" },
                values: new object[] { 1, new DateOnly(2024, 6, 24), 1, 1, "Contract1", 1000m });

            migrationBuilder.InsertData(
                table: "corporate_clients",
                columns: new[] { "IdClient", "CorpoName", "KRS" },
                values: new object[] { 1, "Corp1", 123456789m });

            migrationBuilder.InsertData(
                table: "personal_clients",
                columns: new[] { "IdClient", "Name", "PESEL", "Surname" },
                values: new object[] { 2, "John", 89012345678m, "Doe" });

            migrationBuilder.InsertData(
                table: "ledgers",
                columns: new[] { "IdPayment", "AmountPaid", "IdContract", "PaidOn" },
                values: new object[] { 1, 500m, 1, new DateOnly(1, 1, 1) });

            migrationBuilder.InsertData(
                table: "one_time_payments",
                columns: new[] { "IdContract", "DateTo", "Status", "UpdatePeriod", "Version" },
                values: new object[] { 1, new DateOnly(2025, 6, 24), "Active", 12, "1.0" });

            migrationBuilder.InsertData(
                table: "subscriptions",
                columns: new[] { "IdContract", "RenevalTimeInMonths" },
                values: new object[] { 1, 12 });

            migrationBuilder.CreateIndex(
                name: "IX_contracts_IdClient",
                table: "contracts",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_IdSoftware",
                table: "contracts",
                column: "IdSoftware");

            migrationBuilder.CreateIndex(
                name: "IX_ledgers_IdContract",
                table: "ledgers",
                column: "IdContract");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "corporate_clients");

            migrationBuilder.DropTable(
                name: "discounts");

            migrationBuilder.DropTable(
                name: "ledgers");

            migrationBuilder.DropTable(
                name: "one_time_payments");

            migrationBuilder.DropTable(
                name: "personal_clients");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "softwares");
        }
    }
}
