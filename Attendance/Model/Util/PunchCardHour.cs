
using System;

namespace Attendance.Model.Util
{
    public class PunchCardHour
    {
        /// <summary>
        /// 日期(几号)
        /// </summary>
        public int day { get; set; }

        /// <summary>
        /// 当日首次打卡hour
        /// </summary>
        public int start_hour { get; set; }

        /// <summary>
        /// 当日末次打卡hour
        /// </summary>
        public int end_hour { get; set; }

        /// <summary>
        /// 第一次和最后一次打卡之间时间间隔
        /// </summary>
        public TimeSpan time_of_duration { get; set; }
    }
}
