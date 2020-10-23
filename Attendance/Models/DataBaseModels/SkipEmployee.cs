using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models.DataBaseModels
{
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
    }
}
