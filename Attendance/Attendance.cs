using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Attendance
{
    public class Attendance
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int seq_num { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string card_id { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string job_num { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime inout_time { get; set; }
        /// <summary>
        /// 地点：1大门-进门,2大门-出门
        /// </summary>
        public int place { get; set; }
        /// <summary>
        /// 是否通过：true通过,false未通过
        /// </summary>
        public bool pass { get; set; }
    }
}
