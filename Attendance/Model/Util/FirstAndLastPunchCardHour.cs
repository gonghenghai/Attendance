
namespace Attendance.Model.Util
{
    public class FirstAndLastPunchCardHour
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
    }
}
