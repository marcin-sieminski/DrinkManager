using Microsoft.EntityFrameworkCore.Migrations;

namespace DrinkManagerWeb.Migrations
{
    public partial class ChangedIsReviedPropertyOfDrink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "Drinks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "Drinks");
        }
    }
}
