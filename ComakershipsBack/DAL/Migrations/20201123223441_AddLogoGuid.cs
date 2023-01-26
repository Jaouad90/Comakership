using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddLogoGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Company");

            migrationBuilder.AddColumn<Guid>(
                name: "LogoGuid",
                table: "Company",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoGuid",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
