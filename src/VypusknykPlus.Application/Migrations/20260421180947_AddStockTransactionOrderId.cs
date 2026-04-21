using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTransactionOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "StockTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_OrderId",
                table: "StockTransactions",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_Orders_OrderId",
                table: "StockTransactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_Orders_OrderId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_OrderId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "StockTransactions");
        }
    }
}
