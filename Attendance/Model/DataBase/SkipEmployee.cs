using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Model.DataBase
{
    /// <summary>
    /// 本表存放那些不需要被分析的员工对象,例如保安,特殊人员等
    /// </summary>
    [Table("skip_employee")]
    public class SkipEmployee
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        [Required]
        [Description("卡号")]
        [Column(TypeName = "char(10)")]
        public string card_id { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [Required]
        [Description("工号")]
        [Column(TypeName = "char(5)")]
        public string job_num { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [Description("姓名")]
        [Column(TypeName = "char(30)")]
        public string emp_name { get; set; }
    }
}
