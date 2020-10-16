using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class changesomethings4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "card_id",
                table: "Attendance",
                type: "char(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "card_id",
                table: "Attendance",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(10)",
                oldNullable: true);
        }
    }
}
