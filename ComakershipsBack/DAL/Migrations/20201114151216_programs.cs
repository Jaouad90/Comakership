using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class programs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComakershipProgram_Comakerships_ComakershipId",
                table: "ComakershipProgram");

            migrationBuilder.DropForeignKey(
                name: "FK_ComakershipProgram_Program_ProgramId",
                table: "ComakershipProgram");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Program",
                table: "Program");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComakershipProgram",
                table: "ComakershipProgram");

            migrationBuilder.RenameTable(
                name: "Program",
                newName: "Programs");

            migrationBuilder.RenameTable(
                name: "ComakershipProgram",
                newName: "ComakershipPrograms");

            migrationBuilder.RenameIndex(
                name: "IX_ComakershipProgram_ProgramId",
                table: "ComakershipPrograms",
                newName: "IX_ComakershipPrograms_ProgramId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Programs",
                table: "Programs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComakershipPrograms",
                table: "ComakershipPrograms",
                columns: new[] { "ComakershipId", "ProgramId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ComakershipPrograms_Comakerships_ComakershipId",
                table: "ComakershipPrograms",
                column: "ComakershipId",
                principalTable: "Comakerships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComakershipPrograms_Programs_ProgramId",
                table: "ComakershipPrograms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComakershipPrograms_Comakerships_ComakershipId",
                table: "ComakershipPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_ComakershipPrograms_Programs_ProgramId",
                table: "ComakershipPrograms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Programs",
                table: "Programs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComakershipPrograms",
                table: "ComakershipPrograms");

            migrationBuilder.RenameTable(
                name: "Programs",
                newName: "Program");

            migrationBuilder.RenameTable(
                name: "ComakershipPrograms",
                newName: "ComakershipProgram");

            migrationBuilder.RenameIndex(
                name: "IX_ComakershipPrograms_ProgramId",
                table: "ComakershipProgram",
                newName: "IX_ComakershipProgram_ProgramId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Program",
                table: "Program",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComakershipProgram",
                table: "ComakershipProgram",
                columns: new[] { "ComakershipId", "ProgramId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ComakershipProgram_Comakerships_ComakershipId",
                table: "ComakershipProgram",
                column: "ComakershipId",
                principalTable: "Comakerships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComakershipProgram_Program_ProgramId",
                table: "ComakershipProgram",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
