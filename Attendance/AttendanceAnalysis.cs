using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance
{
    public class AttendanceAnalysis
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 星期 1-7:星期一---星期天
        /// </summary>
        public int week { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string job_num { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 班种类:1早班,2白班,3晚班
        /// </summary>
        public int shift { get; set; }
        /// <summary>
        /// 是否完美(没有晚到,迟到,早退)
        /// </summary>
        public bool perfect { get; set; }
        /// <summary>
        /// 是否仅仅晚到(上班半小时内为晚到)
        /// </summary>
        public bool only_half_late { get; set; }
        /// <summary>
        /// 是否迟到早退(迟到或早退或两者皆有)
        /// </summary>
        public bool late_or_early_leave { get; set; }
        /// <summary>
        /// 是否没到
        /// </summary>
        public bool no_attnd { get; set; }
        /// <summary>
        /// 是否晚到
        /// </summary>
        public bool half_late { get; set; }
        /// <summary>
        /// 晚到时长
        /// </summary>
        public TimeSpan half_late_tm { get; set; }
        /// <summary>
        /// 是否迟到
        /// </summary>
        public bool late { get; set; }
        /// <summary>
        /// 迟到时长
        /// </summary>
        public TimeSpan late_tm { get; set; }
        /// <summary>
        /// 是否早退
        /// </summary>
        public bool early_leave { get; set; }
        /// <summary>
        /// 早退时长
        /// </summary>
        public TimeSpan early_leave_tm { get; set; }
        /// <summary>
        /// 首刷时间
        /// </summary>
        public DateTime first_tm { get; set; }
        /// <summary>
        /// 最后刷时间
        /// </summary>
        public DateTime last_tm { get; set; }
        /// <summary>
        /// 本工作日内刷卡次数(一分钟内多次刷卡视为一次)
        /// </summary>
        public int times { get; set; }
    }
}
