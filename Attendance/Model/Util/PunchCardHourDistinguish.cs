using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance.Model.Util
{
    class PunchCardHourDistinguish
    {
        public List<FirstAndLastPunchCardHour> normal { get; set; }
        public List<FirstAndLastPunchCardHour> only_one { get; set; }
    }
}
