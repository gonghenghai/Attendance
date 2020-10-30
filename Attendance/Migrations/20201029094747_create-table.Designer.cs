﻿// <auto-generated />
using System;
using Attendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Attendance.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20201029094747_create-table")]
    partial class createtable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Attendance.Model.DataBase.AttendanceInfo", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("card_id")
                        .IsRequired()
                        .HasColumnType("char(10)");

                    b.Property<string>("emp_name")
                        .IsRequired()
                        .HasColumnType("char(30)");

                    b.Property<DateTime>("inout_time")
                        .HasColumnType("datetime");

                    b.Property<string>("job_num")
                        .IsRequired()
                        .HasColumnType("char(5)");

                    b.Property<bool>("pass")
                        .HasColumnType("bool");

                    b.Property<byte>("place")
                        .HasColumnType("tinyint");

                    b.Property<int>("seq_num")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("attendance_info");
                });

            modelBuilder.Entity("Attendance.Model.DataBase.HolidayChanges", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("day")
                        .HasColumnType("date");

                    b.Property<string>("type")
                        .IsRequired()
                        .HasColumnType("char(10)");

                    b.HasKey("id");

                    b.ToTable("holiday_changes");
                });

            modelBuilder.Entity("Attendance.Model.DataBase.SkipEmployee", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("card_id")
                        .IsRequired()
                        .HasColumnType("char(10)");

                    b.Property<string>("emp_name")
                        .IsRequired()
                        .HasColumnType("char(30)");

                    b.Property<string>("job_num")
                        .IsRequired()
                        .HasColumnType("char(5)");

                    b.HasKey("id");

                    b.ToTable("skip_employee");
                });
#pragma warning restore 612, 618
        }
    }
}
