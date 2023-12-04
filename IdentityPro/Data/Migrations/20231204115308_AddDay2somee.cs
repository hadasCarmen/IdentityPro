using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ice_cream_shop.Migrations
{
    public partial class AddDay2somee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Order");
        }
    }
}
