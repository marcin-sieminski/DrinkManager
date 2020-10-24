using Microsoft.EntityFrameworkCore.Migrations;

namespace DrinkManagerWeb.Migrations
{
    public partial class AddedDrinkReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drinks_DrinkReview_DrinkReviewId",
                table: "Drinks");

            migrationBuilder.DropIndex(
                name: "IX_Drinks_DrinkReviewId",
                table: "Drinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DrinkReview",
                table: "DrinkReview");

            migrationBuilder.DropColumn(
                name: "DrinkReviewId",
                table: "Drinks");

            migrationBuilder.RenameTable(
                name: "DrinkReview",
                newName: "DrinkReviews");

            migrationBuilder.AddColumn<string>(
                name: "DrinkId",
                table: "DrinkReviews",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DrinkReviews",
                table: "DrinkReviews",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DrinkReviews_DrinkId",
                table: "DrinkReviews",
                column: "DrinkId",
                unique: true,
                filter: "[DrinkId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_DrinkReviews_Drinks_DrinkId",
                table: "DrinkReviews",
                column: "DrinkId",
                principalTable: "Drinks",
                principalColumn: "DrinkId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DrinkReviews_Drinks_DrinkId",
                table: "DrinkReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DrinkReviews",
                table: "DrinkReviews");

            migrationBuilder.DropIndex(
                name: "IX_DrinkReviews_DrinkId",
                table: "DrinkReviews");

            migrationBuilder.DropColumn(
                name: "DrinkId",
                table: "DrinkReviews");

            migrationBuilder.RenameTable(
                name: "DrinkReviews",
                newName: "DrinkReview");

            migrationBuilder.AddColumn<int>(
                name: "DrinkReviewId",
                table: "Drinks",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DrinkReview",
                table: "DrinkReview",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Drinks_DrinkReviewId",
                table: "Drinks",
                column: "DrinkReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drinks_DrinkReview_DrinkReviewId",
                table: "Drinks",
                column: "DrinkReviewId",
                principalTable: "DrinkReview",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
