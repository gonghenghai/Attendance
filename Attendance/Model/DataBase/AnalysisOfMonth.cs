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
        /// <summary>
        /// 自增主键
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "char(5)")]
        public string job_num { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [Description("char(30)")]
        [Column(TypeName = "")]
        public string emp_name { get; set; }

        /// <summary>
        /// 班类型(9:9点班;10:10点班...)
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "char(5)")]
        public string shift { get; set; }

        /// <summary>
        /// 打卡次数
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "int")]
        public int punch_card_count { get; set; }

        /// <summary>
        /// 缺勤打卡天数
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "")]
        public int absence_punch_card_day_count { get; set; }

        /// <summary>
        /// 超勤打卡天数
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "")]
        public int exceed_punch_card_day_count { get; set; }

        /// <summary>
        /// 是否存在长假
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "")]
        public bool exist_long_holiday { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "")]
        public int day_count { get; set; }

        /// <summary>
        /// 是否满勤
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "")]
        public bool full_attendance { get; set; }

        /// <summary>
        /// 差满勤日期列表
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "")]
        public string less_than_workday_list { get; set; }

        /// <summary>
        /// 人员类型推测(普通,新员工/长假归来,非正常)
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "")]
        public string attendance_speculate { get; set; }

        /// <summary>
        /// 受限满勤(新入职的员工，出差的员工等)
        /// </summary>
        [Required]
        [Description("")]
        [Column(TypeName = "")]
        public bool limited_full_attendance { get; set; }

        //public int 

    }
}
