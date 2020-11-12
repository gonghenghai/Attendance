using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Model.DataBase
{
    /// <summary>
    /// 员工每天打卡信息分析结果
    /// </summary>
    [Table("analysis_of_day")]
    public class AnalysisOfDay
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        [Column(TypeName = "int")]
        public int id { get; set; }

        /// <summary>
        /// 年月日
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
        /// 卡号
        /// </summary>
        [Required]
        [Column(TypeName = "char(10)")]
        public string card_id { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [Required]
        [Column(TypeName = "char(5)")]
        public string job_num { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [Column(TypeName = "char(30)")]
        public string emp_name { get; set; }

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
        /// 首刷小时
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int first_hr { get; set; }

        /// <summary>
        /// 末刷小时
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int last_hr { get; set; }

        /// <summary>
        /// 早中班(以中午12点为界,小于等于12点的为早班,大于12点的为中班)
        /// </summary>
        [Required]
        [Column(TypeName = "char(6)")]
        public string shift { get; set; }

        /// <summary>
        /// 打卡小时数
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int punch_card_hour { get; set; }

        /// <summary>
        /// 打卡小时数与标准工作小时数的差值(0表示刚好够小时数,正数表示超出的小时数,负数表示缺少的小时数)
        /// </summary>
        [Required]
        [Column(TypeName = "smallint")]
        public int punch_card_hour_compare { get; set; }

        /// <summary>
        /// 打卡时长
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan punch_card_time { get; set; }

        /// <summary>
        /// 打卡时长与标准工作时长的分钟差值(正的表示超过正常工作时间的分钟数,负的表示少于正常工作时间的分钟数)
        /// </summary>
        [Required]
        [Column(TypeName = "smallint")]
        public int punch_card_minute_compare { get; set; }

        /// <summary>
        /// 是否完美(没有晚到,迟到,早离,早退)
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool perfect { get; set; }

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
        /// 是否早离
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool half_leave_early { get; set; }

        /// <summary>
        /// 早离时长
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan half_leave_early_tm { get; set; }

        /// <summary>
        /// 是否早退
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool leave_early { get; set; }

        /// <summary>
        /// 早退时长
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan leave_early_tm { get; set; }

        //[Required]
        //[Column(TypeName = "time")]
        //public TimeSpan in_company_time { get; set; }

        /// <summary>
        /// 本工作日内打卡次数(所有打卡次数)
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int punch_card_count_total { get; set; }

        /// <summary>
        /// 本工作日内打卡次数(有效打卡次数，排除短时间内多次刷卡的无效数据)
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int punch_card_count_valid { get; set; }
    }
}
