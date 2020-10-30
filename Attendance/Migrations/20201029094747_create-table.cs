using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Attendance.Migrations
{
    public partial class createtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attendance_info",
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
                    table.PrimaryKey("PK_attendance_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "holiday_changes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    day = table.Column<DateTime>(type: "date", nullable: false),
                    type = table.Column<string>(type: "char(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_changes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "skip_employee",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    card_id = table.Column<string>(type: "char(10)", nullable: false),
                    job_num = table.Column<string>(type: "char(5)", nullable: false),
                    emp_name = table.Column<string>(type: "char(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skip_employee", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendance_info");

            migrationBuilder.DropTable(
                name: "holiday_changes");

            migrationBuilder.DropTable(
                name: "skip_employee");
        }
    }
}
