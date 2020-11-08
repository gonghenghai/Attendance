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
            a.ImportAttendanceInfoToMySQL("C:\\Users\\GongHengHai\\Desktop", "test.xls");
            a.ImportHolidayChangesToMySQL("C:\\Users\\GongHengHai\\Desktop", "holiday.xlsx");
            a.ImportSkipEmployeeToMySQL("C:\\Users\\GongHengHai\\Desktop", "skip.xlsx");
            //analysisMain b = new analysisMain();
            //b.AnalysisResultsToMySQL();
        }
    }
}
