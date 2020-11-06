using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Model.DataBase
{
    /// <summary>
    /// 员工打卡信息月度分析结果
    /// </summary>
    [Table("analysis_of_month")]
    public class AnalysisOfMonth
    {
        public int id { get; set; }

        [Required]
        [Description("年月日(日为1)")]
        [Column(TypeName = "datetime")]
        public DateTime date { get; set; }

        [Required]
        [Description("卡号")]
        [Column(TypeName = "char(10)")]
        public string card_id { get; set; }

        [Required]
        [Description("工号")]
        [Column(TypeName = "char(5)")]
        public string job_num { get; set; }

        [Required]
        [Description("char(30)")]
        [Column(TypeName = "姓名")]
        public string emp_name { get; set; }

        [Required]
        [Description("本月内所有打卡次数")]
        [Column(TypeName = "smallint")]
        public int punch_card_count_month { get; set; }

        [Required]
        [Description("打卡日里的平均打卡次数")]
        [Column(TypeName = "tinyint")]
        public int punch_card_count_day { get; set; }

        [Required]
        [Description("本月在公司总时间")]
        [Column(TypeName = "time")]
        public TimeSpan attendance_time_of_month { get; set; }

        [Required]
        [Description("所有正常打卡日(至少两次打卡)在公司的平均时间")]
        [Column(TypeName = "time")]
        public TimeSpan attenadance_time_of_day { get; set; }

        [Required]
        [Description("缺勤打卡天数")]
        [Column(TypeName = "tinyint")]
        public int absence_punch_card_day_count { get; set; }

        [Required]
        [Description("超勤打卡天数")]
        [Column(TypeName = "tinyint")]
        public int exceed_punch_card_day_count { get; set; }

        [Required]
        [Description("缺勤日期列表")]
        [Column(TypeName = "char(70)")]
        public string absence_punch_card_day_list { get; set; }

        [Required]
        [Description("超勤日期列表")]
        [Column(TypeName = "char(30)")]
        public string exceed_punch_card_day_list { get; set; }

        [Required]
        [Description("是否有只打一次卡的日期")]
        [Column(TypeName = "bool")]
        public bool has_only_one_punch_card_day { get; set; }

        [Required]
        [Description("只打一次卡的日期列表")]
        [Column(TypeName = "char(30)")]
        public string only_one_punch_card_day_list { get; set; }

        [Required]
        [Description("早打卡小时去重列表")]
        [Column(TypeName = "char(45)")]
        public string start_hour_unique_list { get; set; }

        [Required]
        [Description("晚打卡小时去重列表")]
        [Column(TypeName = "char(45)")]
        public string end_hour_unique_list { get; set; }

        [Required]
        [Description("早打卡小时变更次数")]
        [Column(TypeName = "tinyint")]
        public int start_hour_change_times { get; set; }

        [Required]
        [Description("晚打卡小时变更次数")]
        [Column(TypeName = "tinyint")]
        public int end_hour_change_times { get; set; }

        [Required]
        [Description("是否满勤")]
        [Column(TypeName = "bool")]
        public bool full_attendance { get; set; }

        [Required]
        [Description("人员类型推测(普通,新员工/长假归来,非正常)")]
        [Column(TypeName = "char(12)")]
        public string role_speculate { get; set; }

        [Required]
        [Description("受限满勤(新入职的员工，出差的员工等)")]
        [Column(TypeName = "bool")]
        public bool limited_full_attendance { get; set; }

        [Required]
        [Description("连续未打卡次数(日期上连续的算作一次)")]
        [Column(TypeName = "tinyint")]
        public int absence_sections_count { get; set; }

        [Required]
        [Description("是否存在长假")]
        [Column(TypeName = "bool")]
        public bool exist_long_holiday { get; set; }
    }
}
