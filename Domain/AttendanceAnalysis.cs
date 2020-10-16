using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class AttendanceAnalysis
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
        /// 班种类:1早班,2白班,3晚班
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
        /// 本工作日内刷卡次数(一分钟内多次刷卡视为一次)
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int times { get; set; }
    }
}
