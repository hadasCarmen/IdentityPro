using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ice_cream_shop.Migrations
{
    public partial class UpdateWeatherInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Humidity",
                table: "Weather",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "Weather",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Temp",
                table: "Weather",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "Weather");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "Weather");

            migrationBuilder.DropColumn(
                name: "Temp",
                table: "Weather");
        }
    }
}
