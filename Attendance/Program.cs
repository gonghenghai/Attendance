using Attendance.Functions;

namespace Attendance
{
    class Program
    {
        static void Main(string[] args)
        {
            Methods methods = new Methods();
            //methods.ImportAttendanceInfoToMySQL();
            methods.AnalysisResultsToMySQL();
            //methods.ImportHolidayChangesToMySQL();
            //methods.ImportSkipEmployeeToMySQL();
        }
    }
}
