using Microsoft.EntityFrameworkCore.Migrations;

namespace EstateInvestmentWebApplication.Data.Migrations
{
    public partial class ver02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "News",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "EstateProjects",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visible",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "EstateProjects");
        }
    }
}
