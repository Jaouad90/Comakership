using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddStudentProgram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgramId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProgramId",
                table: "Users",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Programs_ProgramId",
                table: "Users",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Programs_ProgramId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProgramId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "Users");
        }
    }
}
