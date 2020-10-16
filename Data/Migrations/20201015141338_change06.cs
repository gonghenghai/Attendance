using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Data.Migrations
{
    public partial class change06 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.CreateTable(
                name: "AttendanceInfo",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    seq_num = table.Column<int>(type: "int", nullable: false),
                    card_id = table.Column<string>(type: "char(10)", nullable: false),
                    job_num = table.Column<string>(type: "char(5)", nullable: false),
                    emp_name = table.Column<string>(type: "char(30)", nullable: false),
                    inout_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    place = table.Column<byte>(type: "tinyint", nullable: false),
                    pass = table.Column<bool>(type: "bool", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceInfo", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceInfo");

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    card_id = table.Column<string>(type: "char(10)", nullable: false),
                    emp_name = table.Column<string>(type: "char(30)", nullable: false),
                    inout_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    job_num = table.Column<string>(type: "char(5)", nullable: false),
                    pass = table.Column<bool>(type: "bool", nullable: false),
                    place = table.Column<byte>(type: "tinyint", nullable: false),
                    seq_num = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.id);
                });
        }
    }
}
