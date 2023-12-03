using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ice_cream_shop.Migrations
{
    public partial class UpdateIsHoliday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHoliday",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHoliday",
                table: "Order");
        }
    }
}
