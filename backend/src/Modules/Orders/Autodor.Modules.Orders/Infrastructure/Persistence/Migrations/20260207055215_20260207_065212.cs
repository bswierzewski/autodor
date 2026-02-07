using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Autodor.Modules.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _20260207_065212 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "orders");

            migrationBuilder.CreateTable(
                name: "ExcludedOrderItems",
                schema: "orders",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ItemNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcludedOrderItems", x => new { x.OrderId, x.ItemNumber });
                });

            migrationBuilder.CreateTable(
                name: "ExcludedOrders",
                schema: "orders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcludedOrders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcludedOrderItems",
                schema: "orders");

            migrationBuilder.DropTable(
                name: "ExcludedOrders",
                schema: "orders");
        }
    }
}
