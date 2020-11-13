using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance.Model.Util
{
    class AttendanceNormalAndShort
    {
        public TimeSpan attendance_time_of_day_normal { get; set; }
        public int attendance_time_of_day_normal_count { get; set; }
        public TimeSpan attendance_time_of_day_short { get; set; }
        public int attendance_time_of_day_short_count { get; set; }
    }
}
