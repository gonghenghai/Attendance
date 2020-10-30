using System.Collections.Generic;

namespace Attendance.Model.Util
{
    public class AnalysisOfMonthData
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int year { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        public int month { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string card_id { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string job_num { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string emp_name { get; set; }

        /// <summary>
        /// 本月一共打卡次数
        /// </summary>
        public int punch_card_count_month { get; set; }

        /// <summary>
        /// 大于两次打卡日期列表
        /// </summary>
        public List<int> day_list { get; set; }

        /// <summary>
        /// 早晚打卡小时值
        /// </summary>
        public List<FirstAndLastPunchCardHour> first_and_last_punch_card_hour_list { get; set; }
    }
}
