using Attendance.Model.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attendance
{
    public class Context:DbContext
    {
        public Context()
        {
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(ConsoleLoggerFactory)
                .EnableSensitiveDataLogging()
                .UseMySQL("Server=localhost; Port=3306; Database=employee; Uid=root; Pwd=123456; Charset=utf8mb4;");
        }

        public DbSet<SkipEmployee> SkipEmployee { get; set; }
        public DbSet<HolidayChanges> HolidayChanges { get; set; }
        public DbSet<AttendanceInfo> AttendanceInfo { get; set; }
        //public DbSet<AnalysisOfMonth> AnalysisOfMonth { get; set; }
        //public DbSet<AnalysisOfDay> AnalysisOfDay { get; set; }

        public static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information
            ).AddConsole();
        });

    }
}
