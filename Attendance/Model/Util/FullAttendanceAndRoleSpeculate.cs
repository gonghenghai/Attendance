using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance.Model.Util
{
    public class FullAttendanceAndRoleSpeculate
    {
        public bool full_attendance { get; set; }
        public string role_speculate { get; set; }
        public bool limited_full_attendance { get; set; }
        public List<List<int>> absence_sections { get; set; }
        public bool long_holiday { get; set; }
    }
}
