using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddReviewsAndPrivateTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Company_CompanyId",
                table: "Review");

            migrationBuilder.DropTable(
               name: "Review");

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentUserId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)

                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Users_UserId",
                        column: x => x.StudentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Review_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_CompanyId",
                table: "Review",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_StudentUserId",
                table: "Review",
                column: "StudentUserId");

            migrationBuilder.AddColumn<int>(
                name: "PrivateTeamId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PrivateTeamId",
                table: "Users",
                column: "PrivateTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Teams_PrivateTeamId",
                table: "Users",
                column: "PrivateTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Teams_PrivateTeamId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PrivateTeamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrivateTeamId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PrivateTeamId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Review_StudentUserId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentUser = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_CompanyId",
                table: "Review",
                column: "CompanyId");
        }
    }
}
