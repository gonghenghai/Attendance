using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Model.DataBase
{
    /// <summary>
    /// 员工每天打卡信息分析结果
    /// </summary>
    public class AnalysisOfDay
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        /// <summary>
        /// 星期 1-7:星期一---星期天
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int week { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [Required]
        [Column(TypeName = "char(5)")]
        public string job_num { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        [Required]
        [Column(TypeName = "char(30)")]
        public string emp_name { get; set; }

        /// <summary>
        /// 班种类:6:六点班，7:七点班，8:八点班...
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int shift { get; set; }

        /// <summary>
        /// 是否完美(没有晚到,迟到,早退)
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool perfect { get; set; }

        /// <summary>
        /// 是否仅仅晚到(上班半小时内为晚到)
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool only_half_late { get; set; }

        /// <summary>
        /// 是否迟到早退(迟到或早退或两者皆有)
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool late_or_early_leave { get; set; }

        /// <summary>
        /// 是否没到
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool no_attnd { get; set; }

        /// <summary>
        /// 是否晚到
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool half_late { get; set; }

        /// <summary>
        /// 晚到时长
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan half_late_tm { get; set; }

        /// <summary>
        /// 是否迟到
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool late { get; set; }

        /// <summary>
        /// 迟到时长
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan late_tm { get; set; }

        /// <summary>
        /// 是否早退
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool early_leave { get; set; }

        /// <summary>
        /// 早退时长
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan early_leave_tm { get; set; }

        /// <summary>
        /// 首刷时间
        /// </summary>
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime first_tm { get; set; }

        /// <summary>
        /// 最后刷时间
        /// </summary>
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime last_tm { get; set; }

        /// <summary>
        /// 在公司总时长(第一次打卡和最后一次打卡时间间隔)
        /// </summary>
        [Required]
        [Column(TypeName ="time")]
        public TimeSpan cmp_tm_total { get; set; }

        /// <summary>
        /// 在公司时长(排除外出的时间)
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan cmp_tm_valid { get; set; }

        /// <summary>
        /// 本工作日内刷卡次数(有效打卡次数，排除短时间内多次刷卡的无效数据)
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int times_valid { get; set; }

        /// <summary>
        /// 本工作日内刷卡次数(所有打卡次数)
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int times_total { get; set; }
    }
}
