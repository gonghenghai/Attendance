using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models.DataBaseModels
{
    public class HolidayChanges
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 非常理化的日期(正常情况下是周六周日放假，节假日视为非常理化日期)
        /// </summary>
        [Required]
        [Column(TypeName ="date")]
        public DateTime day { get; set; }
        /// <summary>
        /// 非常理化类型：加班或放假
        /// </summary>
        [Required]
        [Column(TypeName = "char(10)")]
        public string type { get; set; }

    }
}
