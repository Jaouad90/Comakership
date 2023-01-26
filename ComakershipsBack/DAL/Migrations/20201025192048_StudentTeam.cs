using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class StudentTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Teams",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentTeams",
                columns: table => new
                {
                    StudentUserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTeams", x => new { x.StudentUserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_StudentTeams_Users_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentTeams_TeamId",
                table: "StudentTeams",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentTeams");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Teams");
        }
    }
}
