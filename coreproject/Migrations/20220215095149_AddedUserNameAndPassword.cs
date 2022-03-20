using Microsoft.EntityFrameworkCore.Migrations;

namespace coreproject.Migrations
{
    public partial class AddedUserNameAndPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Accounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Accounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Accounts");
        }
    }
}
