using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Attendance.Migrations
{
    public partial class changeinttodouble5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analysis_of_day",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    week = table.Column<byte>(type: "tinyint", nullable: false),
                    card_id = table.Column<string>(type: "char(10)", nullable: false),
                    job_num = table.Column<string>(type: "char(5)", nullable: false),
                    emp_name = table.Column<string>(type: "char(30)", nullable: false),
                    first_tm = table.Column<DateTime>(type: "datetime", nullable: false),
                    last_tm = table.Column<DateTime>(type: "datetime", nullable: false),
                    first_hr = table.Column<byte>(type: "tinyint", nullable: false),
                    last_hr = table.Column<byte>(type: "tinyint", nullable: false),
                    shift = table.Column<string>(type: "char(6)", nullable: false),
                    punch_card_hour = table.Column<byte>(type: "tinyint", nullable: false),
                    punch_card_hour_compare = table.Column<short>(type: "smallint", nullable: false),
                    punch_card_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    punch_card_minute_compare = table.Column<short>(type: "smallint", nullable: false),
                    perfect = table.Column<bool>(type: "bool", nullable: false),
                    half_late = table.Column<bool>(type: "bool", nullable: false),
                    half_late_tm = table.Column<TimeSpan>(type: "time", nullable: false),
                    late = table.Column<bool>(type: "bool", nullable: false),
                    late_tm = table.Column<TimeSpan>(type: "time", nullable: false),
                    half_leave_early = table.Column<bool>(type: "bool", nullable: false),
                    half_leave_early_tm = table.Column<TimeSpan>(type: "time", nullable: false),
                    leave_early = table.Column<bool>(type: "bool", nullable: false),
                    leave_early_tm = table.Column<TimeSpan>(type: "time", nullable: false),
                    punch_card_count_total = table.Column<byte>(type: "tinyint", nullable: false),
                    punch_card_count_valid = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis_of_day", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "analysis_of_month",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    card_id = table.Column<string>(type: "char(10)", nullable: false),
                    job_num = table.Column<string>(type: "char(5)", nullable: false),
                    emp_name = table.Column<string>(type: "char(30)", nullable: false),
                    punch_card_day_count = table.Column<byte>(type: "tinyint", nullable: false),
                    attendance_time_of_month = table.Column<TimeSpan>(type: "time", nullable: false),
                    attenadance_time_of_day = table.Column<TimeSpan>(type: "time", nullable: false),
                    absence_punch_card_day_count = table.Column<byte>(type: "tinyint", nullable: false),
                    absence_punch_card_day_list = table.Column<string>(type: "char(70)", nullable: true),
                    exceed_punch_card_day_count = table.Column<byte>(type: "tinyint", nullable: false),
                    exceed_punch_card_day_list = table.Column<string>(type: "char(30)", nullable: true),
                    has_only_one_punch_card_day = table.Column<bool>(type: "bool", nullable: false),
                    only_one_punch_card_day_list = table.Column<string>(type: "char(30)", nullable: true),
                    start_hour_unique_list = table.Column<string>(type: "char(45)", nullable: true),
                    start_hour_change_times = table.Column<byte>(type: "tinyint", nullable: false),
                    end_hour_unique_list = table.Column<string>(type: "char(45)", nullable: true),
                    end_hour_change_times = table.Column<byte>(type: "tinyint", nullable: false),
                    role_speculate = table.Column<string>(type: "char(12)", nullable: true),
                    full_attendance = table.Column<bool>(type: "bool", nullable: false),
                    limited_full_attendance = table.Column<bool>(type: "bool", nullable: false),
                    absence_sections_count = table.Column<byte>(type: "tinyint", nullable: false),
                    exist_long_holiday = table.Column<bool>(type: "bool", nullable: false),
                    punch_card_count_month = table.Column<short>(type: "smallint", nullable: false),
                    punch_card_count_day = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis_of_month", x => x.id);
                });

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
                name: "analysis_of_day");

            migrationBuilder.DropTable(
                name: "analysis_of_month");

            migrationBuilder.DropTable(
                name: "attendance_info");

            migrationBuilder.DropTable(
                name: "holiday_changes");

            migrationBuilder.DropTable(
                name: "skip_employee");
        }
    }
}
