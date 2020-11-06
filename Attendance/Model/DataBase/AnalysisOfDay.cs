using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Model.DataBase
{
    /// <summary>
    /// 员工每天打卡信息分析结果
    /// </summary>
    public class AnalysisOfDay
    {
        public int id { get; set; }

        [Required]
        [Description("年月日")]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        [Required]
        [Description("星期 1-7:星期一---星期天")]
        [Column(TypeName = "tinyint")]
        public int week { get; set; }

        [Required]
        [Description("卡号")]
        [Column(TypeName = "char(10)")]
        public string card_id { get; set; }

        [Required]
        [Description("工号")]
        [Column(TypeName = "char(5)")]
        public string job_num { get; set; }

        [Required]
        [Description("姓名")]
        [Column(TypeName = "char(30)")]
        public string emp_name { get; set; }

        [Required]
        [Description("首刷时间")]
        [Column(TypeName = "datetime")]
        public DateTime first_tm { get; set; }

        [Required]
        [Description("最后刷时间")]
        [Column(TypeName = "datetime")]
        public DateTime last_tm { get; set; }

        [Required]
        [Description("首刷小时")]
        [Column(TypeName = "tinyint")]
        public int first_hr { get; set; }

        [Required]
        [Description("末刷小时")]
        [Column(TypeName = "tinyint")]
        public int last_hr { get; set; }

        [Required]
        [Description("早中班")]
        [Column(TypeName = "char(6)")]
        public string shift { get; set; }

        [Required]
        [Description("打卡小时数")]
        [Column(TypeName = "tinyint")]
        public int punch_card_hour { get; set; }

        [Required]
        [Description("打卡小时数与标准工作小时数的差值(0表示刚好够小时数,正表示超出小时数,负表示缺少小时数)")]
        [Column(TypeName = "tinyint")]
        public int punch_card_hour_compare { get; set; }

        [Required]
        [Description("打卡时长")]
        [Column(TypeName = "time")]
        public TimeSpan punch_card_time { get; set; }

        [Required]
        [Description("打卡时长与标准工作时长的分钟差值(正的表示超过正常工作时间的分钟数,负的表示少于正常工作时间的工作时长)")]
        [Column(TypeName = "smallint")]
        public int punch_card_minute_compare { get; set; }

        [Required]
        [Description("是否完美(没有晚到,迟到,早离,早退)")]
        [Column(TypeName = "bool")]
        public bool perfect { get; set; }

        [Required]
        [Description("是否晚到")]
        [Column(TypeName = "bool")]
        public bool half_late { get; set; }

        [Required]
        [Description("晚到时长")]
        [Column(TypeName = "time")]
        public TimeSpan half_late_tm { get; set; }

        [Required]
        [Description("是否迟到")]
        [Column(TypeName = "bool")]
        public bool late { get; set; }

        [Required]
        [Description("迟到时长")]
        [Column(TypeName = "time")]
        public bool late_tm { get; set; }

        [Required]
        [Description("是否早离")]
        [Column(TypeName = "bool")]
        public bool half_leave_early { get; set; }

        [Required]
        [Description("早离时长")]
        [Column(TypeName = "time")]
        public bool half_leave_early_tm { get; set; }

        [Required]
        [Description("是否早退")]
        [Column(TypeName = "bool")]
        public bool leave_early { get; set; }

        [Required]
        [Description("早退时长")]
        [Column(TypeName = "time")]
        public bool leave_early_tm { get; set; }

        [Required]
        [Description("在公司时长(排除外出的时间)")]
        [Column(TypeName = "time")]
        public TimeSpan in_company_time { get; set; }

        [Required]
        [Description("本工作日内打卡次数(所有打卡次数)")]
        [Column(TypeName = "tinyint")]
        public int punch_card_count_total { get; set; }

        [Required]
        [Description("本工作日内打卡次数(有效打卡次数，排除短时间内多次刷卡的无效数据)")]
        [Column(TypeName = "tinyint")]
        public int punch_card_count_valid { get; set; }
    }
}
