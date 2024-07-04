using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm.DataAccess.Migrations
{
    public partial class saved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SavedAddresses",
                table: "ShoppingCarts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavedAddresses",
                table: "ShoppingCarts");
        }
    }
}
