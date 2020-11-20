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
            a.ImportAttendanceInfoToMySQL("D:", "files/test.xlsx");
            a.ImportHolidayChangesToMySQL("D:", "files/holiday.xlsx");
            a.ImportSkipEmployeeToMySQL("D:", "files/skip.xlsx");
            //analysisMain b = new analysisMain();
            //b.AnalysisResultsToMySQL();
        }
    }
}
