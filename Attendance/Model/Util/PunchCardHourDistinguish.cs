using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance.Model.Util
{
    public class PunchCardHourDistinguish
    {
        public List<PunchCardHour> normal { get; set; }
        public List<PunchCardHour> only_one { get; set; }
        public List<PunchCardHour> all { get; set; }
    }
}
