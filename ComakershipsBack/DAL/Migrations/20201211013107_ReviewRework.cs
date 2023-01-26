using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class ReviewRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Review",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ForCompany",
                table: "Review",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ForCompany",
                table: "Review");

        }
    }
}
