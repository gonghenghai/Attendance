using System;
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
        /// <summary>
        /// 主键
        /// </summary>
        [Column(TypeName = "int")]
        public int id { get; set; }

        /// <summary>
        /// 年月日(日为1)
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }

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
        /// 本月内所有打卡天数(包括只打一次卡的)
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int punch_card_day_count { get; set; }

        /// <summary>
        /// 本月总打卡时间(本月每天的初次打卡和末次打卡时间差之和)
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan attendance_time_of_month { get; set; }

        /// <summary>
        /// 所有正常打卡日(至少两次打卡)打卡平均时间
        /// </summary>
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan attenadance_time_of_day { get; set; }

        /// <summary>
        /// 缺勤打卡天数
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int absence_punch_card_day_count { get; set; }

        /// <summary>
        /// 缺勤日期列表
        /// </summary>
        [Column(TypeName = "char(70)")]
        public string absence_punch_card_day_list { get; set; }

        /// <summary>
        /// 超勤打卡天数
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int exceed_punch_card_day_count { get; set; }

        /// <summary>
        /// 超勤日期列表
        /// </summary>
        [Column(TypeName = "char(30)")]
        public string exceed_punch_card_day_list { get; set; }

        /// <summary>
        /// 是否有只打一次卡的日期
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool has_only_one_punch_card_day { get; set; }

        /// <summary>
        /// 只打一次卡的日期列表
        /// </summary>
        [Column(TypeName = "char(30)")]
        public string only_one_punch_card_day_list { get; set; }

        /// <summary>
        /// 早打卡小时去重列表
        /// </summary>
        [Column(TypeName = "char(45)")]
        public string start_hour_unique_list { get; set; }

        /// <summary>
        /// 早打卡小时变更次数
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int start_hour_change_times { get; set; }

        /// <summary>
        /// 晚打卡小时去重列表
        /// </summary>
        [Column(TypeName = "char(45)")]
        public string end_hour_unique_list { get; set; }

        /// <summary>
        /// 晚打卡小时变更次数
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int end_hour_change_times { get; set; }

        /// <summary>
        /// 人员类型推测(普通,新员工/长假归来,非正常)
        /// </summary>
        [Column(TypeName = "char(12)")]
        public string role_speculate { get; set; }

        /// <summary>
        /// 是否满勤
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool full_attendance { get; set; }

        /// <summary>
        /// 受限满勤(新入职的员工，出差的员工等)
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool limited_full_attendance { get; set; }

        /// <summary>
        /// 连续未打卡次数(日期上连续的算作一次)
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int absence_sections_count { get; set; }

        /// <summary>
        /// 是否存在长假
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool exist_long_holiday { get; set; }

        /// <summary>
        /// 本月内所有打卡次数(包括一天只打一次卡的)
        /// </summary>
        [Required]
        [Column(TypeName = "smallint")]
        public int punch_card_count_month { get; set; }

        /// <summary>
        /// 打卡日里的平均打卡次数(包括一天只打一次卡的)
        /// </summary>
        [Required]
        [Column(TypeName = "double")]
        public double punch_card_count_day { get; set; }
    }
}
