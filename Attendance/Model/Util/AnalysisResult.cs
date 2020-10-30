using Attendance.Model.DataBase;
using System.Collections.Generic;

namespace Attendance.Model.Util
{
    public class AnalysisResult
    {
        public List<AnalysisOfMonth> analysis_of_month { get; set; }
        public List<AnalysisOfDay> analysis_of_day { get; set; }
    }
}
