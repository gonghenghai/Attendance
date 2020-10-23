using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models.DataBaseModels
{
    public class AttendanceInfo
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Required]
        [Column(TypeName ="int")]
        public int seq_num { get; set; }
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
        /// 时间
        /// </summary>
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime inout_time { get; set; }
        /// <summary>
        /// 地点：51:大门-进门,52:大门-出门,61:6F大门-进门,62:6F大门-出门,71:7F大门-进门,72:7F大门-出门,0:未知
        /// </summary>
        [Required]
        [Column(TypeName = "tinyint")]
        public int place { get; set; }
        /// <summary>
        /// 是否通过：true通过,false未通过
        /// </summary>
        [Required]
        [Column(TypeName = "bool")]
        public bool pass { get; set; }
    }
}
