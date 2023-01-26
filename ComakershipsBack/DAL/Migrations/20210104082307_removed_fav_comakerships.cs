using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class removed_fav_comakerships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comakerships_Users_StudentUserId",
                table: "Comakerships");

            migrationBuilder.DropIndex(
                name: "IX_Comakerships_StudentUserId",
                table: "Comakerships");

            migrationBuilder.DropColumn(
                name: "StudentUserId",
                table: "Comakerships");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentUserId",
                table: "Comakerships",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comakerships_StudentUserId",
                table: "Comakerships",
                column: "StudentUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comakerships_Users_StudentUserId",
                table: "Comakerships",
                column: "StudentUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
