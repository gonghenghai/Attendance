using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance.Model.Util
{
    public class Configuration
    {
        //长假天数(包括)
        public static int long_holiday = 5;
        //早班标准工作小时,目前来说都是9
        public static int standard_work_hour_of_morning = 9;
        //中班标准工作小时,中班很少有人能坚持到九个小时,所以可以灵活配置,比如降低到8
        public static int standard_work_hour_of_noon = 8;
        //初次打卡缓冲分钟数.举例:这样将使得9:00到9:05分以内的时间被统一视为9:00
        public static int arrive_buffer_minutes = 5;
        //最后打卡缓冲分钟数.举例:这样将使得5:55到6:00的时间被统一视为6:00
        public static int leave_buffer_minutes = 5;
        //初次打卡边界分钟数,这个分钟数将用来判定晚到,早离.举例:9:05到9:25将被视为晚到,但不算是迟到,超过9:25的就算是迟到了
        public static int arrive_limit_minutes = 25;
        //最后打卡边界分钟数,这个分钟数将用来判定迟到,早退.举例:5:45到5:55将被视为早离,但不算是早退,但在5:45之前的就算是早退了
        public static int leave_limit_minutes = 15;
        //打卡间隔秒数,小于这个的视为短时间重复打卡将被去重
        public static int punch_card_intervel_seconds = 60;
        //每天正常打卡时间和较短打卡时间分界值的小时数,两种时间将被分别归类
        public static int attendance_normal_short_distinguish_hours = 7;
        //每天正常打卡时间和较短打卡时间分界值的分钟数,两种时间将被分别归类
        public static int attendance_normal_short_distinguish_minutes = 0;
    }
}
