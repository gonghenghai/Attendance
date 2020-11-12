using Attendance.Controller.Analysis;
using Attendance.Controller.ExcelToSQL;

namespace Attendance
{
    class Program
    {
        static void Main(string[] args)
        {
            //testclass method = new testclass();
            //method.test();
            excelToSQLMain a = new excelToSQLMain();
            a.ImportAttendanceInfoToMySQL("D:", "test.xlsx");
            a.ImportHolidayChangesToMySQL("D:", "holiday.xlsx");
            a.ImportSkipEmployeeToMySQL("D:", "skip.xlsx");
            analysisMain b = new analysisMain();
            b.AnalysisResultsToMySQL();
        }
    }
}
