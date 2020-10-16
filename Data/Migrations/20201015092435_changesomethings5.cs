using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class changesomethings5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "week",
                table: "AttendanceAnalysis",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "times",
                table: "AttendanceAnalysis",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "shift",
                table: "AttendanceAnalysis",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "perfect",
                table: "AttendanceAnalysis",
                type: "bool",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "only_half_late",
                table: "AttendanceAnalysis",
                type: "bool",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "no_attnd",
                table: "AttendanceAnalysis",
                type: "bool",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "late_or_early_leave",
                table: "AttendanceAnalysis",
                type: "bool",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "late",
                table: "AttendanceAnalysis",
                type: "bool",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "job_num",
                table: "AttendanceAnalysis",
                type: "char(5)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "half_late",
                table: "AttendanceAnalysis",
                type: "bool",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "emp_name",
                table: "AttendanceAnalysis",
                type: "char(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "early_leave",
                table: "AttendanceAnalysis",
                type: "bool",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date",
                table: "AttendanceAnalysis",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<byte>(
                name: "place",
                table: "Attendance",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "pass",
                table: "Attendance",
                type: "bool",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "job_num",
                table: "Attendance",
                type: "char(5)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "emp_name",
                table: "Attendance",
                type: "char(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "card_id",
                table: "Attendance",
                type: "char(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(10)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "week",
                table: "AttendanceAnalysis",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "times",
                table: "AttendanceAnalysis",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "shift",
                table: "AttendanceAnalysis",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<short>(
                name: "perfect",
                table: "AttendanceAnalysis",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AlterColumn<short>(
                name: "only_half_late",
                table: "AttendanceAnalysis",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AlterColumn<short>(
                name: "no_attnd",
                table: "AttendanceAnalysis",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AlterColumn<short>(
                name: "late_or_early_leave",
                table: "AttendanceAnalysis",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AlterColumn<short>(
                name: "late",
                table: "AttendanceAnalysis",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AlterColumn<string>(
                name: "job_num",
                table: "AttendanceAnalysis",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(5)");

            migrationBuilder.AlterColumn<short>(
                name: "half_late",
                table: "AttendanceAnalysis",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AlterColumn<string>(
                name: "emp_name",
                table: "AttendanceAnalysis",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(30)");

            migrationBuilder.AlterColumn<short>(
                name: "early_leave",
                table: "AttendanceAnalysis",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date",
                table: "AttendanceAnalysis",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "place",
                table: "Attendance",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<short>(
                name: "pass",
                table: "Attendance",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AlterColumn<string>(
                name: "job_num",
                table: "Attendance",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(5)");

            migrationBuilder.AlterColumn<string>(
                name: "emp_name",
                table: "Attendance",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(30)");

            migrationBuilder.AlterColumn<string>(
                name: "card_id",
                table: "Attendance",
                type: "char(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(10)");
        }
    }
}
