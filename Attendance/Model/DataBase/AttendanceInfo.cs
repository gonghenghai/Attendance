using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Model.DataBase
{
    /// <summary>
    /// 员工打卡信息,这些数据都是直接从excel读取的
    /// </summary>
    [Table("attendance_info")]
    public class AttendanceInfo
    {
        public int id { get; set; }

        [Required]
        [Description("序号")]
        [Column(TypeName ="int")]
        public int seq_num { get; set; }

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
        [Description("时间")]
        [Column(TypeName = "datetime")]
        public DateTime inout_time { get; set; }

        [Required]
        [Description("地点：51:大门-进门,52:大门-出门,61:6F大门-进门,62:6F大门-出门,71:7F大门-进门,72:7F大门-出门,0:未知")]
        [Column(TypeName = "tinyint")]
        public int place { get; set; }

        [Required]
        [Description("是否通过：true通过,false未通过")]
        [Column(TypeName = "bool")]
        public bool pass { get; set; }
    }
}
