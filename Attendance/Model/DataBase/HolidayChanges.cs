using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Model.DataBase
{
    /// <summary>
    /// 本表存放那些特殊日期包括放假和加班
    /// </summary>
    [Table("holiday_changes")]
    public class HolidayChanges
    {
        public int id { get; set; }

        [Required]
        [Description("非常理化的日期(正常情况下周六周日放假其余上班,不符合这个的都是非常理化日期,包括放假和加班)")]
        [Column(TypeName ="date")]
        public DateTime day { get; set; }

        [Required]
        [Description("非常理化类型：加班或放假")]
        [Column(TypeName = "char(10)")]
        public string type { get; set; }

    }
}
