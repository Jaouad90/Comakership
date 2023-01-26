using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddApplicationsAndStudentsToComakership : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Teams_Comakerships_ComakershipId",
            //    table: "Teams");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Users_Comakerships_ComakershipId",
            //    table: "Users");

            //migrationBuilder.DropIndex(
            //    name: "IX_Users_ComakershipId",
            //    table: "Users");

            //migrationBuilder.DropIndex(
            //    name: "IX_Teams_ComakershipId",
            //    table: "Teams");

            //migrationBuilder.DropColumn(
            //    name: "ComakershipId",
            //    table: "Users");

            //migrationBuilder.DropColumn(
            //    name: "ComakershipId",
            //    table: "Teams");

            migrationBuilder.CreateTable(
                name: "TeamComakership",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false),
                    ComakershipId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamComakership", x => new { x.ComakershipId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_TeamComakership_Comakerships_ComakershipId",
                        column: x => x.ComakershipId,
                        principalTable: "Comakerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamComakership_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserComakership",
                columns: table => new
                {
                    StudentUserId = table.Column<int>(nullable: false),
                    ComakershipId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserComakership", x => new { x.ComakershipId, x.StudentUserId });
                    table.ForeignKey(
                        name: "FK_UserComakership_Comakerships_ComakershipId",
                        column: x => x.ComakershipId,
                        principalTable: "Comakerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserComakership_Users_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamComakership_TeamId",
                table: "TeamComakership",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserComakership_StudentUserId",
                table: "UserComakership",
                column: "StudentUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamComakership");

            migrationBuilder.DropTable(
                name: "UserComakership");

            migrationBuilder.AddColumn<int>(
                name: "ComakershipId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComakershipId",
                table: "Teams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ComakershipId",
                table: "Users",
                column: "ComakershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ComakershipId",
                table: "Teams",
                column: "ComakershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Comakerships_ComakershipId",
                table: "Teams",
                column: "ComakershipId",
                principalTable: "Comakerships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Comakerships_ComakershipId",
                table: "Users",
                column: "ComakershipId",
                principalTable: "Comakerships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
