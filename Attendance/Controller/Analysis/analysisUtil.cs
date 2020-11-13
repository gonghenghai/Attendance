using Attendance.Model.DataBase;
using Attendance.Model.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Attendance.Controller.Analysis
{
    class analysisUtil
    {
        /// <summary>
        /// 获取某个月份的工作日列表
        /// </summary>
        /// <param name="Year">目标年份</param>
        /// <param name="Month">目标月份</param>
        /// <returns></returns>
        public static List<int> GetWorkDayListOfMonth(int year, int month)
        {
            DateTime firstDay = new DateTime(year, month, 1);

            //获取本月节假日，调班等特殊日期对象
            List<HolidayChanges> holidays = new List<HolidayChanges>();
            List<HolidayChanges> overtimes = new List<HolidayChanges>();
            using (var context = new Context())
            {
                holidays = context.HolidayChanges.Where(x => x.type == "放假" && x.day >= firstDay.Date && x.day < firstDay.Date.AddMonths(1)).OrderBy(x => x.day).AsNoTracking().ToList();
                overtimes = context.HolidayChanges.Where(x => x.type == "加班" && x.day >= firstDay.Date && x.day < firstDay.Date.AddMonths(1)).OrderBy(x => x.day).AsNoTracking().ToList();
            }

            //将特殊日期值存入列表
            List<int> holidays_day_list = holidays.Select(t => t.day.Day).ToList();
            List<int> overtimes_day_list = overtimes.Select(t => t.day.Day).ToList();

            //工作日List
            List<int> workDayList = new List<int>();
            //用来遍历本月的工具对象
            DateTime day = firstDay;

            //获取理论上的工作日(仅排除周末)
            for (int i = 1; i <= 31; i++)
            {
                //利用AddDays方法可以确保遍历本月所有的日期而不会报错，但是有可能会出现
                //下个月的日期，而下个月的日期由于星期的不同会污染数据，所以用Month加以甄别
                if (day.Month == month)
                {
                    if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
                        workDayList.Add(i);
                }
                day = day.AddDays(1);
            }

            //删除所有假期日期
            workDayList.RemoveAll(x => holidays_day_list.Contains(x));
            //添加所有加班日期
            foreach (var overtime in overtimes_day_list)
            {
                workDayList.Add(overtime);
            }

            //理顺日期列表并返回
            workDayList.Sort();
            return workDayList;
        }

        /// <summary>
        /// 获取某年某月里所有周末的日期列表(去掉周末加班的)
        /// </summary>
        /// <param name="year">目标年份</param>
        /// <param name="month">目标月份</param>
        /// <returns></returns>
        public static List<int> GetWeekendDayListOfMonth(int year, int month)
        {
            List<int> weekend_day_list = new List<int>();
            DateTime day = new DateTime(year, month, 1);

            for (int i = 1; i <= 31; i++)
            {
                //避免统计到下个月的数据
                if (day.Month == month)
                {
                    if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                        weekend_day_list.Add(i);
                }
                day = day.AddDays(1);
            }

            List<HolidayChanges> overtimes = new List<HolidayChanges>();
            using (var context = new Context())
            {
                overtimes = context.HolidayChanges.Where(x => x.type == "加班" && x.day >= day.Date && x.day < day.Date.AddMonths(1)).OrderBy(x => x.day).AsNoTracking().ToList();
            }
            List<int> overtimes_list = overtimes.Select(t => t.day.Day).ToList();

            //去掉加班的日期列表
            weekend_day_list.Except(overtimes_list);
            return weekend_day_list;
        }

        /// <summary>
        /// 获取员工月度早晚打卡小时数据
        /// </summary>
        /// <param name="one_emp_month_infos">目标员工对象一个月的打卡数据</param>
        /// <returns></returns>
        public static PunchCardHourDistinguish GetFirstAndLastPunchCardHour(List<AttendanceInfo> one_emp_month_infos)
        {
            //本月所有早晚打卡小时值(正常的,有两次以上打卡)
            List<PunchCardHour> punch_card_hour_list_normal = new List<PunchCardHour>();
            //本月所有早晚打卡小时值(不正常的,只有一次打卡记录)
            List<PunchCardHour> punch_card_hour_list_only_one = new List<PunchCardHour>();
            //本月所有早晚打卡小时值
            List<PunchCardHour> punch_card_hour_list_all = new List<PunchCardHour>();

            List<List<AttendanceInfo>> one_emp_day_infos_group = GetSliceOfDayFromMonth(one_emp_month_infos.First().inout_time.Date, one_emp_month_infos);
            TimeSpan timespan = new TimeSpan();
            foreach (var one_emp_day_infos in one_emp_day_infos_group)
            {
                DateTime first_time = one_emp_day_infos.First().inout_time;
                DateTime last_time = one_emp_day_infos.Last().inout_time;

                PunchCardHour punch_card_hour = new PunchCardHour();
                punch_card_hour.day = first_time.Day;

                //当天第一次打卡时间的小时值(分钟值:0-30视为本小时,31-59.59视为下一个小时)
                timespan = first_time.TimeOfDay;
                punch_card_hour.start_hour = timespan.Minutes % 60 > 30 ? timespan.Hours + 1 : timespan.Hours;

                //当天最后一次打卡时间的小时值(分钟值:0-30视为本小时,31-59.59视为下一个小时)
                timespan = last_time.TimeOfDay;
                punch_card_hour.end_hour = timespan.Minutes % 60 > 30 ? timespan.Hours + 1 : timespan.Hours;

                //最后一次打卡和第一次打卡之间的时间间隔
                timespan = last_time.TimeOfDay - first_time.TimeOfDay;
                punch_card_hour.time_of_duration = timespan;

                //假如最后打卡时间过了午夜,会导致时间为负数,这里做下处理
                if (last_time.Day > first_time.Day)
                    punch_card_hour.time_of_duration += new TimeSpan(24, 0, 0);


                if (one_emp_day_infos.Count >= 2)
                    punch_card_hour_list_normal.Add(punch_card_hour);
                else
                    punch_card_hour_list_only_one.Add(punch_card_hour);

                punch_card_hour_list_all.Add(punch_card_hour);
            }

            PunchCardHourDistinguish result = new PunchCardHourDistinguish();
            result.normal = punch_card_hour_list_normal;
            result.only_one = punch_card_hour_list_only_one;
            result.all = punch_card_hour_list_all;
            return result;
        }

        public static AttendanceNormalAndShort GetAttendanceNormalAndShort(List<PunchCardHour> normal)
        {
            AttendanceNormalAndShort result = new AttendanceNormalAndShort();

            //这两个是可配置的,将来会从外界读取
            int hour = Configuration.attendance_normal_short_distinguish_hours;
            int minute = Configuration.attendance_normal_short_distinguish_minutes;
            //边界时间,这个将被用来区分正常时间和非正常时间
            TimeSpan limit = new TimeSpan(hour,minute,0);
            foreach (var val in normal)
            {
                if (val.time_of_duration >= limit)
                {
                    result.attendance_time_of_day_normal += val.time_of_duration;
                    result.attendance_time_of_day_normal_count++;
                }
                else 
                {
                    result.attendance_time_of_day_short += val.time_of_duration;
                    result.attendance_time_of_day_short_count++;
                }
            }
            if (result.attendance_time_of_day_normal_count>0)
                result.attendance_time_of_day_normal /= result.attendance_time_of_day_normal_count;
            if(result.attendance_time_of_day_short_count>0)
                result.attendance_time_of_day_short /= result.attendance_time_of_day_short_count;
            return result;
        }

        public static List<List<AttendanceInfo>> GetSliceOfDayFromMonth(DateTime date,List<AttendanceInfo> one_emp_month_infos)
        {
            List<List<AttendanceInfo>> result = new List<List<AttendanceInfo>>();
            
            //用于切片每天的时间，以早上四点为临界点，每天的时间段为凌晨四点到次日凌晨四点
            DateTime start_time = new DateTime(date.Year, date.Month, 1, 4, 0, 0);
            DateTime end_time = new DateTime(date.Year, date.Month, 2, 4, 0, 0);
            for(int i = 1; i <= 31; i++)
            {
                //员工一天内的所有打卡数据
                List<AttendanceInfo>  one_emp_day_infos = one_emp_month_infos.Where(x => x.inout_time > start_time && x.inout_time < end_time).ToList();
                //至少要有一次打卡记录,例如正常的周末就是没有打卡记录的
                if (one_emp_day_infos.Count > 0)
                {
                    result.Add(one_emp_day_infos);
                }

                //跳转到下一天
                start_time = start_time.AddDays(1);
                end_time = end_time.AddDays(1);
            }

            return result;
        }

        /// <summary>
        /// 获取shift
        /// </summary>
        /// <param name="first_hr">当天第一次打卡的小时值</param>
        /// <returns></returns>
        public static string GetShift(int first_hr)
        {
            string shift = "早班";
            if (first_hr >12)
            {
                shift = "中班";
            }
            return shift;
        }

        /// <summary>
        /// 获取日打卡分析的一些数据
        /// </summary>
        /// <param name="first_tm">当天第一次打卡时间</param>
        /// <param name="last_tm">当天最后一次打卡时间</param>
        /// <param name="first_hr">当天第一次打卡小时值</param>
        /// <param name="last_hr">当天最后一次打卡小时值</param>
        /// <param name="standard_work_hour">标准工作时间(为区分早中班留缓冲)</param>
        /// <returns></returns>
        public static FirstAndLastPunchCardAnalysisResult GetFirstAndLastPunchCardAnalysis(DateTime first_tm, DateTime last_tm, int first_hr,int last_hr,string shift)
        {
        //这块的数值将来都将从配置文件里面读取,以便于个性化配置
            //标准工作小时,目前来说都是9
            int standard_work_hour = Configuration.standard_work_hour_of_morning;
            //晚班很少有人能坚持到九个小时,所以可以灵活配置,比如降低到8
            if (shift == "中班") standard_work_hour = Configuration.standard_work_hour_of_noon;

            //初次打卡缓冲分钟数.举例:这样将使得9:00到9:05分以内的时间被统一视为9:00
            int arrive_buffer_minutes = Configuration.arrive_buffer_minutes;
            //最后打卡缓冲分钟数.举例:这样将使得5:55到6:00的时间被统一视为6:00
            int leave_buffer_minutes = Configuration.leave_buffer_minutes;
            //初次打卡边界分钟数,这个分钟数将用来判定晚到,早离.举例:9:05到9:25将被视为晚到,但不算是迟到,超过9:25的就算是迟到了
            int arrive_limit_minutes = Configuration.arrive_limit_minutes;
            //最后打卡边界分钟数,这个分钟数将用来判定迟到,早退.举例:5:45到5:55将被视为早离,但不算是早退,但在5:45之前的就算是早退了
            int leave_limit_minutes = Configuration.leave_limit_minutes;

            FirstAndLastPunchCardAnalysisResult result = new FirstAndLastPunchCardAnalysisResult();

            TimeSpan standard_work_time = new TimeSpan(standard_work_hour, 0, 0);
            int punch_card_hour = last_hr - first_hr;
            TimeSpan punch_card_time = last_tm - first_tm;
            int punch_card_hour_compare = punch_card_hour - standard_work_hour;
            int punch_card_minute_compare = (int)(punch_card_time - standard_work_time).TotalMinutes;

            result.punch_card_hour = punch_card_hour;
            result.punch_card_time = punch_card_time;
            result.punch_card_hour_compare = punch_card_hour_compare;
            result.punch_card_minute_compare = punch_card_minute_compare;

            #region 时间差值,用于接下来的迟到早退等非正常打卡的判断和相关时长的统计
            int year = first_tm.Year;
            int month = first_tm.Month;
            int day = first_tm.Day;
            int day_last = last_tm.Day;//最后打卡时间可能会到下一天,所以不能简单的都用第一次打卡的日期
            TimeSpan first_tm_gap =  first_tm - new DateTime(year, month, day, first_hr,0,0);//晚到或迟到多长时间
            //这里可能会出现last_hr等于24小时的情况,所以用AddHours来添加小时数
            TimeSpan last_tm_gap = new DateTime(year, month, day_last, 0,0,0).AddHours(last_hr) - last_tm;//早离或早退多长时间

            TimeSpan standard_first_tm_gap = first_tm - new DateTime(year, month,day, 9,0,0);//早班最迟标准时间下的晚到或迟到多长时间
            TimeSpan standard_last_tm_gap = new DateTime(year, month,day, 9 + standard_work_hour,0,0)- last_tm;//早班最迟标准时间下的早离或早退多长时间
            if (shift == "中班") 
            {
                standard_first_tm_gap = first_tm - new DateTime(year, month, day, 15, 0, 0);//中班最迟标准时间下的晚到或迟到多长时间
                //这里可能会出现15+standard_work_hour等于24小时的情况,所以用AddHours来添加小时数
                standard_last_tm_gap = new DateTime(year, month, day, 15, 0, 0).AddHours(standard_work_hour) - last_tm;//中班最迟标准时间下的早离或早退多长时间   
            }
            #endregion

            TimeSpan arrive_buffer = new TimeSpan(0, arrive_buffer_minutes, 0);
            TimeSpan leave_buffer = new TimeSpan(0, leave_buffer_minutes, 0);
            TimeSpan arrive_limit = new TimeSpan(0, arrive_limit_minutes, 0);
            TimeSpan leave_limit = new TimeSpan(0, leave_limit_minutes, 0);

            #region
            bool first_tm_buffer_gap_meet = first_tm_gap <= arrive_buffer; //初次打卡时间与相邻小时的差值低于设置的忽略值
            bool first_tm_limit_gap_meet = first_tm_gap > arrive_buffer && first_tm_gap <= arrive_limit; //初次打卡时间与相邻小时的差值大于设置的忽略值小于设置的临界值
            bool first_tm_limit_gap_not_meet = first_tm_gap > arrive_limit; //初次打卡时间与相邻小时的差值大于设置的临界值 

            bool last_tm_buffer_gap_meet = last_tm_gap <= leave_buffer; //最后打卡时间与相邻小时的差值低于设置的忽略值
            bool last_tm_limit_gap_meet = last_tm_gap > leave_buffer && last_tm_gap <= leave_limit; //最后打卡时间与相邻小时的差值大于设置的忽略值小于设置的临界值
            bool last_tm_limit_gap_not_meet = last_tm_gap > leave_limit; //最后打卡时间与相邻小时的差值大于设置的临界值

            bool standard_last_tm_buffer_gap_meet = standard_last_tm_gap <= leave_buffer; //在最迟标准初始打卡时间下与相邻小时的差值低于设置的忽略值
            bool standard_last_tm_limit_gap_meet = standard_last_tm_gap > leave_buffer && standard_last_tm_gap <= leave_limit;//在最迟标准初始打卡时间下与相邻小时的差值大于设置的忽略值小于设置的临界值
            bool standard_last_tm_limit_gap_not_meet = standard_last_tm_gap > leave_limit; //在最迟标准初始打卡时间下与相邻小时的差值大于设置的临界值

            bool enough_hour_equal = punch_card_hour == standard_work_hour;
            bool enough_hour_more = punch_card_hour > standard_work_hour;
            bool enough_hour_equal_more = punch_card_hour >= standard_work_hour;
            #endregion

            bool first_hour_in_range = FirstHourInRange(shift, first_hr);

            /*
            初始判断
            假设以30分为划分线获取的小时值代表准确的小时信息:
            初始打卡时间为早上7:26,默认判定为迟到26分钟,忽略早到34分钟,早到1小时34分钟等情况
            初始打卡时间为早上8:35,默认判定为早到25分钟,忽略迟到35分钟,迟到1小时35分钟等情况
            初始打卡时间为下午2:28,默认判定为迟到28分钟,忽略早到32分钟的情况
            也就是说,这里的判断都假设其初始打卡时间可以准确推断其应该的初始打卡小时.

            判断逻辑:

            完美 初始时间在正确区间, 小时够, first_tm_buffer_gap_meet,last_tm_buffer_gap_meet

            晚到 初始时间在正确区间, first_tm_limit_gap_meet                        (时长:first_tm_gap)

            迟到 初始时间在正确区间,first_tm_limit_gap_not_meet                     (时长:first_tm_gap)
            迟到 初始时间不在正确区间                                               (时长:standard_first_tm_gap)

            早离 初始时间在正确区间,小时数刚好, last_tm_limit_gap_meet              (时长:last_tm_gap)
            早离 初始时间不在正确区间,standard_last_tm_limit_gap_meet               (时长:standard_last_tm_gap)

            早退 初始时间在正确区间,小时数刚好,last_tm_limit_gap_not_meet           (时长:last_tm_gap)
            早退 初始时间在正确区间,小时数不足                                      (时长:last_tm_gap)
            早退 初始时间不在正确区间,standard_last_tm_limit_gap_not_meet           (时长:standard_last_tm_gap)
             */

            if (first_hour_in_range)
            {
                if (enough_hour_equal_more)
                {
                    if (first_tm_buffer_gap_meet && last_tm_buffer_gap_meet)
                    {
                        result.perfect = true;
                    }
                }
                else
                {
                    result.leave_early = true;
                    result.leave_early_tm= last_tm_gap;
                }
                if (first_tm_limit_gap_meet)
                {
                    result.half_late = true;
                    result.half_late_tm = first_tm_gap;
                }
                if (first_tm_limit_gap_not_meet)
                {
                    result.late = true;
                    result.late_tm = first_tm_gap;
                }
                if (enough_hour_equal)
                {
                    if (last_tm_limit_gap_meet)
                    {
                        result.half_leave_early = true;
                        result.half_leave_early_tm = last_tm_gap;
                    }
                    if (last_tm_limit_gap_not_meet)
                    {
                        result.leave_early = true;
                        result.leave_early_tm = last_tm_gap;
                    }
                }
            }
            else
            {
                result.late = true;
                result.late_tm = standard_first_tm_gap;
                if (standard_last_tm_limit_gap_meet)
                {
                    result.half_leave_early = true;
                    result.half_leave_early_tm = standard_last_tm_gap;
                }
                if (standard_last_tm_limit_gap_not_meet)
                {
                    result.leave_early = true;
                    result.leave_early_tm = standard_last_tm_gap;
                }
            }

            /*
            再判断
            过滤掉初始判断中可能因早到导致的误判:
            例如:
            初始打卡时间为早上7:26,默认判定为迟到26分钟,忽略早到34分钟,早到1小时34分钟等情况
            初始打卡时间为下午2:28,默认判定为迟到28分钟,忽略早到32分钟的情况

            可行性:
            假如一个人昨天和今天都是早上7:25打卡,出于大判断,其将被归纳为早班,即其所属的班范围包括6:00到9:00,任何一个都是可能的.
            假设此员工前一天的班为7:00,今天的班为8:00,则此人在昨天应该被归纳为迟到,今天被应该被归纳为早到.
            即基于某一天的单一时间无法对其做出准确的判断,任何判断都是有可能错有可能对的.

            补救手段:
            做出保守推断,在条件允许下,允许晚到,排除迟到.
            对于在xx:25--xx:30这个时间段的,如果可以往下推算,例如7:28推算到8:00,8:28推算到9:00,那么就向下推算.
            但是这样有可能会导致很蠢的结果:
            例如一个员工工作时间应该为:8:00-17:00
            其打卡时间为8:28-16:50,其应该是迟到28分钟,早离10分钟,如果往下推的话,就会导致结果为:
            早到32分钟,早退1小时10分钟.
            和较为模糊的结果:
            例如一个员工工作时间应该为:8:00-17:00
            其打卡时间为8:28-17:20,其应该是迟到28分钟,晚走20分钟,如果往下推的话,就会导致结果为:
            早到32分钟,早退40分钟.

            问:假如一个人正常班是8:00,但是由于其早上迟到了,8:28打的卡,由于其知道自己迟到,因此晚上较正常时间晚走,这可以被允许吗?
            答:这个是应该被允许的:1.由于允许某天单独调班的存在,所以哪怕我们可以确定这天的打卡较平常有些不同,我们也无法断定这不是调班所导致的.
                                  2.工时已经够了,且在被允许的初始打卡小时段内,从情理上也应该允许.
            问:但是假如此员工没有晚走,或晚走的时间不够,该如何判断?例如正常班为8:00,迟到打卡为8:28,被顺延到了9:00,其离开的时间也被顺延到了18:00,
               但是其并没有到18:00走,而是5:28走的,该如何判断?
            答:此时将会产生两种结果:1.迟到28分钟,晚走28分钟,2.早到32分钟,早退32分钟.
               从正常逻辑来说,一个人最后打卡的时间较为能够反映其兜底的正常时间.即:一个人可能晚走,甚至晚走两三个小时,但是几乎很少有人会提前走超过半个小时.
               这时的判断逻辑将变为:允许此人通过晚走来抹去其迟到的行为,但是晚走的时间必须为位于早离区间,如果处于早退区间,将视为其自动放弃此项权利.
               所以结果将是:1.迟到28分钟,晚走28分钟.

            所以核心逻辑是:允许用最后打卡时间的富裕对初始打卡时间进行补贴,但是不得使得最后打卡时间位于早退区间.因为其实起核心参考作用的还是最后打卡时间

            所以按如下逻辑进行再判断,对同时满足如下条件的进行过滤:
            1.初始判断中首次打卡位于晚到或迟到区间
            2.打卡小时可以往下顺延
            3.顺延后的最后打卡时间不在早退区间
             */

            //这里之所以要专门写这个是因为last_tm_gap有可能是大于一小时的,这样一来last_tm_gap_min就只能取到它的小数部分,
            //只能用(int)last_tm_gap.TotalMinutes来取分钟数
            int last_tm_gap_min = (int)last_tm_gap.TotalMinutes;

            if (first_tm_limit_gap_meet || first_tm_limit_gap_not_meet)
            {
                if (FirstHourInRange(shift, first_hr + 1))
                {
                    /*
                    last_tm_gap_min代表早离早退的分钟数,如果是晚走的话,其值应该是负数,想获得晚走的分钟数就要用:-last_tm_gap_min
                    -last_tm_gap_min-60>=-leave_buffer_minutes意为:
                    晚走时间的分钟数减去60分钟,也就是回退一小时,然后看是否大于缓冲分钟数的负数.
                    例如:
                    打卡时间为:下午6:58
                    last_tm_gap为:-00:58:00
                    -last_tm_gap_min-60为:58-60=-2
                    -2是落在-5到0之间的
                     */
                    if (-last_tm_gap_min-60>=-leave_buffer_minutes)
                    {
                        //肯定没有迟到晚到了,所以数据清空.也没有早离早退,所以不用添加数据.
                        result.late = false;
                        result.late_tm = new TimeSpan(0, 0, 0);
                        result.half_late = false;
                        result.half_late_tm = new TimeSpan(0, 0, 0);
                    }
                    else if (-last_tm_gap_min - 60 < -leave_buffer_minutes && -last_tm_gap_min - 60 >= -leave_limit_minutes)
                    {
                        //没有迟到晚到,但是有早离了
                        result.late = false;
                        result.late_tm = new TimeSpan(0, 0, 0);
                        result.half_late = false;
                        result.half_late_tm = new TimeSpan(0, 0, 0);

                        result.half_leave_early = true;
                        result.half_leave_early_tm = new TimeSpan(0, 60 - last_tm_gap_min, 0);
                    }
                }
            }

            /*
            再判断
            过滤掉初始判断中可能因迟到导致的误判(迟到半小时以上)
            例如:
            初始打卡时间为早上8:35,默认判定为早到25分钟,忽略迟到35分钟,迟到1小时35分钟等情况

            由于允许打卡时间顺延,通过晚走来抹去晚到迟到等行为.
            所以对于此类迟到半小时以上导致的误判,且可以顺延的行为,只有当早退时间过长,且可合理回退的时候才会被考虑.
            例如:
            一个员工正常班时为8:00--17:00.
            1.真实打卡时间为8:35--17:35
              正确应该是迟到35分钟,晚走35分钟.会被误判为早到25分钟,早退25分钟.
              这时候尝试将初始打卡小时前调,改为8:00,获得迟到35分钟和35分钟结余,补贴给最后打卡时间后获得:迟到35分钟,晚走35分钟
            2.真实打卡时间为8:35--16:45
              会被误判为早到25分钟,早退1小时15分钟.
              尝试初始打卡前调,获得35分钟结余,补贴最后打卡时间后获得:迟到35分钟,早离15分钟
            3.真实打卡时间为8:35--16:44
              会被误判为早到25分钟,早退1小时16分钟.
              尝试初始打卡前调,获得35分钟结余,补贴最后打卡时间后获得:迟到35分钟,早退16分钟
            4.真实打卡时间为8:35--16:00
              会被误判为早到25分钟,早退2小时.
              尝试初始打卡前调,获得35分钟结余,补贴最后打卡时间后获得:迟到35分钟,早退1小时
              这时候就放弃补贴,回退为早到25分钟,早退2小时.
              之所以这样设计是基于以下逻辑:出现早退一小时和早退两小时其实没有本质的区别了,应该都是有事情提前离开.
              

            所以核心逻辑是:允许用初始打卡时间对最后打卡时间进行补贴,虽然这会导致初始打卡时间位于迟到区间,但是这能够使得最后打卡时间更为合理,因为最后
                           打卡时间是更为可靠的判断依据,但是倘若最后打卡时间已经无法挽救,即最后打卡时间过于超前,则会放弃补贴.

            对同时满足如下条件的进行过滤:
            1.最后打卡时间位于早退区间
            2.初始打卡时间有结余(早到)
            3.提前初始打卡小时使得其转为迟到后所获得的结余可以使最后打卡时间脱离早退区间(15分钟或其他设置的分钟数)或小于一小时.
             */

            if (last_tm_limit_gap_not_meet)
            {
                if (first_tm_gap.Minutes < 0)
                {
                    /*
                    last_tm_gap_min代表早离早退的分钟数,想获得晚走的分钟数的话就要用负数:-last_tm_gap_min
                    -last_tm_gap_min + 60代表:晚走的分钟数加上后移的一个小时的分钟数
                    例如:
                    打卡时间为:下午5:35
                    -last_tm_gap_min + 60=-25+60=35
                    表示晚走35分钟
                    打卡时间为:下午4:50
                    -last_tm_gap_min + 60=-70+60=-10
                    表示早离10分钟
                    打卡时间为:下午4:58
                    -last_tm_gap_min + 60=-62+60=-2
                    表示离规定时间差两分钟走,不算早离,算准时离开.
                     */
                    if (-last_tm_gap_min + 60 >= -leave_buffer_minutes)
                    {
                        /*
                         现在肯定迟到了,因为想让first_tm_gap.Minutes < 0只能是打卡时间在打卡小时的左边半小时内    
                         之前肯定是早到,现在肯定是迟到半个小时以上
                         */
                        result.late = true;
                        result.late_tm = new TimeSpan(0, 60 - (-first_tm_gap.Minutes), 0);

                        //不早退了,数据清空
                        result.leave_early = false;
                        result.leave_early_tm = new TimeSpan(0, 0, 0);
                    }
                    else if (-last_tm_gap_min + 60 < -leave_buffer_minutes && -last_tm_gap_min + 60 >= -leave_limit_minutes)
                    {
                        result.late = true;
                        result.late_tm = new TimeSpan(0, 60 - (-first_tm_gap.Minutes), 0);

                        //不早退,但是早离
                        result.half_leave_early = true;
                        //-(-last_tm_gap_min+60)=last_tm_gap_min-60
                        //-last_tm_gap_min+60:早走的分钟数,落在-15到-5的区间,早离的时间为其分钟数取正
                        //也可以理解为:之前早退的分钟数减去一个小时后,仍然落在5-15的区间,其早退的分钟数也就是之前的分钟数减60
                        result.half_leave_early_tm = new TimeSpan(0, last_tm_gap_min - 60, 0);
                        result.leave_early = false;
                        result.leave_early_tm = new TimeSpan(0, 0, 0);
                    }else if (-last_tm_gap_min + 60>-60)
                    {
                        result.late = true;
                        result.late_tm = new TimeSpan(0, 60 - (-first_tm_gap.Minutes), 0);

                        //之前早退的分钟数减60
                        result.leave_early_tm = new TimeSpan(0, last_tm_gap_min - 60, 0);
                    }
                }
            }

            return result;            
        }

        /// <summary>
        /// 判断初始打卡小时值是否落在合理区间
        /// 防止通过时长合规来绕过迟到的监控,但是它对于早退并没有监控的功能
        /// 对于迟到只有保底的监控功能.譬如一个人的正常初始打卡时间应该是八点钟,但是他今天选择了九点钟打卡,这样的变化不会被归结到迟到范畴
        /// 因为没有办法判断他是否发生了班类型的变化,比如说昨天是八点的班,但是今天就是九点的班,这种变化是可能存在且被允许的
        /// 为什么无法监控早退,原因是必须综合初次打卡的小时值和最后打卡的小时值,判断的逻辑没有被放在这里.
        /// 而且譬如说,一个人正常的打卡小时应该是早上七点到下午四点,但是他今天是早上九点打卡和下午四点打卡,按照正常的逻辑应该是迟到和正常离开
        /// 但是由于还是不知道他今天是不是调班了,因为他突然有一天调班的可能性是存在的,那么判断就会变为早上九点为正常打卡,下午四点为早退
        /// 由于早退的判断逻辑是依赖于初始打卡,但是初始打卡存在模糊性,这就决定了无法准确判断是否早退.例如你说下午三点是早退吗?但是假如他是上午六点
        /// 上班的,就不是早退.
        /// 所以这里的判断逻辑就是初始打卡时间允许有范围的活动,以中午十二点为界,上午的必须挂靠于六点,七点,八点,九点,最早为六点,最迟为九点
        /// 下午的必须挂靠于两点,三点,对于一点自动跳到两点,因为一点是休息时间,所以不可能有一点的班.
        /// 所以逻辑就是初始打卡时间可以伸缩,在合理范围内的迟到将会自动视为调整到下一个工作区间,最后打卡小时将会被视为早退的判断依据,哪怕他理应是
        /// 迟到加正常离开.
        /// </summary>
        /// <param name="shift">早班/中班</param>
        /// <param name="first_hr">初始打卡小时值</param>
        /// <param name="last_hr">最后打卡小时值</param>
        /// <returns></returns>
        public static bool FirstHourInRange(string shift,int first_hr)
        {
            bool right_range = true;

            List<int> start_up_hour_out = new List<int>() { 10, 11, 12};
            List<int> start_down_hour = new List<int>() { 13, 14, 15 };

            if (shift == "早班")
            {
                //早班能够监控的时间只有初始打卡时间,必须保证其小于等于9
                //不然的话假如10点打卡,就仍然可以通过晚走来实现打卡正常
                //而至于几点走则无法控制,他愿意晚上十一点走也可以
                if (start_up_hour_out.Contains(first_hr))
                {
                    right_range = false;
                }
            }else if (shift == "中班")
            {
                //初始打卡时间必须在下午一点到三点之间
                if (!start_down_hour.Contains(first_hr))
                {
                    right_range = false;
                }
            }

            return right_range;
        }

        public static int GetValidPunchCardCount(List<AttendanceInfo> day_infos)
        {
            int count = day_infos.Count;
            TimeSpan previous_time = new TimeSpan(0,0,0);
            TimeSpan interval = new TimeSpan(0,Configuration.punch_card_intervel_seconds,0);

            foreach (var info in day_infos)
            {
                //这里是为了避免由于过了午夜导致结果为负数从而小于1分钟,所以这里要取正值
                if ((info.inout_time.TimeOfDay - previous_time).Duration() <= interval)
                {
                    count--;
                }
                previous_time = info.inout_time.TimeOfDay;
            }

            return count;
        }

        //public static TimeSpan GetInCompanyTime(List<AttendanceInfo> day_infos)
        //{
        //    TimeSpan time = new TimeSpan(0,0,0);

        //    foreach (var info in day_infos)
        //    {

        //    }

        //    return time;
        //}

        public static bool Perfect(DateTime first_tm,DateTime last_tm,int first_hr)
        {
            bool perfect = true;

            TimeSpan work_time = new TimeSpan(9,0,0);
            if (last_tm - first_tm < work_time)
            {
                perfect = false;
            }
            List<int> start_hour = new List<int>() { 6,7,8,9,14,15 };
            if (!start_hour.Contains(first_hr))
            {
                perfect = false;
            }

            return perfect;
        }

        public static StartAndEenHourChangeTimes GetStartAndEenHourChangeTimes(List<PunchCardHour> normal)
        {
            StartAndEenHourChangeTimes result = new StartAndEenHourChangeTimes();

            int start_hour_change_times = 0;
            int end_hour_change_times = 0;
            for(int i = 0; i < normal.Count-1; i++)
            {
                if (normal[i + 1].start_hour - normal[i].start_hour != 0)
                    start_hour_change_times++;

                if (normal[i + 1].end_hour - normal[i].end_hour != 0)
                    end_hour_change_times++;
            }
            result.start_hour_change_times = start_hour_change_times;
            result.end_hour_change_times = end_hour_change_times;
            return result;
        }

        /// <summary>
        /// 是否存在之前的月度分析数据以确定是否是新员工
        /// </summary>
        /// <param name="job_num">工号</param>
        /// <param name="emp_name">姓名</param>
        public static bool RecruitsOrNot(string job_num,string emp_name)
        {
            bool result=false;
            using(var context=new Context())
            {
                int count = context.AnalysisOfMonth.Where(x => x.job_num == job_num && x.emp_name == emp_name).Count();
                result= count==0?true:false;
            }
            return result;
        }

        /// <summary>
        /// 是否是长假归来的员工
        /// </summary>
        /// <param name="job_num">工号</param>
        /// <param name="emp_name">姓名</param>
        /// <param name="emp_name">本月从一号开始未打卡的天数</param>
        /// <returns></returns>
        public static bool LongHolidayReturnBackFromThisMonth(string job_num,string emp_name,DateTime date_time,int count)
        {
            bool result = false;
            using (var context = new Context())
            {
                //以前有他的月度分析记录
                List<AnalysisOfMonth> analysis_of_month_list = context.AnalysisOfMonth.Where(x => x.job_num == job_num && x.emp_name == emp_name).OrderBy(t => t.date).AsNoTracking().ToList();

                if (analysis_of_month_list.Count > 0)
                {
                    AnalysisOfMonth last_one = analysis_of_month_list.Last();
                    //如果上个月的月度打卡记录分析直接为空的话
                    if(last_one.date<new DateTime(date_time.Year, date_time.Month-1,1))
                    {
                        return true;
                    }
                    //获取上个月未打卡的日期列表
                    List<int> less_than_workday_list = last_one.absence_punch_card_day_list.Split(",").Select(Int32.Parse).ToList();
                    //获取上个月的休息日列表
                    List<int> weekend_day_list = GetWeekendDayListOfMonth(date_time.Year,date_time.Month);
                    //获取上个月工作日列表
                    List<int> workday_list = GetWorkDayListOfMonth(date_time.Year, date_time.Month);

                    List<List<int>> less_than_wrokday_sections_list = GetWorkdaysContinuousSections(less_than_workday_list,weekend_day_list);
                    List<int> last_holiday = less_than_wrokday_sections_list.Last();
                    //假如最后一个未打卡日期列表切片里面包含工作日的最后一天,且切片天数和下个月从一号开始未打卡天数
                    //加在一起大于10天,就判定他属于请了长假,或是出差之类的,将他划归为归来人员
                    if (last_holiday.Contains(workday_list.Last()) && LongHolidayJudge(last_holiday.Count + count))
                    {
                        return true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取是否满勤以及身份推测等相关信息
        /// </summary>
        /// <param name="less_than_workday_list">未打卡日期列表</param>
        /// <param name="weekend_day_list">周末日期列表</param>
        /// <param name="job_num">工号</param>
        /// <param name="emp_name">姓名</param>
        /// <param name="date_time"></param>
        /// <returns></returns>
        public static FullAttendanceAndRoleSpeculate GetFullAttendanceAndRoleSpeculate(List<int> less_than_workday_list, List<int> weekend_day_list,string job_num,string emp_name, DateTime date_time)
        {
            FullAttendanceAndRoleSpeculate result = new FullAttendanceAndRoleSpeculate();
            result.full_attendance = false;
            result.limited_full_attendance = false;
            result.role_speculate = "普通";
            result.absence_sections = new List<List<int>>();
            result.long_holiday = false;

            //所有工作日都打卡的是满勤
            if (less_than_workday_list.Count == 0)
            {
                result.full_attendance = true;
            }
            else
            {
                //获取未打卡日期去除掉周末干扰后的连续日期的切片
                List<List<int>> outer_list = GetWorkdaysContinuousSections(less_than_workday_list, weekend_day_list);

                foreach(var list in outer_list)
                {
                    if (LongHolidayJudge(list.Count))
                        result.long_holiday = true;
                }

                result.absence_sections = outer_list;

                //对切片进行分析
                //假如有五个以上的碎片化没打卡时间,这个有可能是特殊人员
                if (outer_list.Count() >= 5)
                {
                    result.role_speculate = "特殊员工";
                }

                //如果只有一个假期切片,可以做进一步分析
                if (outer_list.Count == 1)
                {
                    //是否包含一号是一个非常重要的判断依据
                    if (outer_list[0].Contains(1))
                    {
                        //如果是新员工
                        if (RecruitsOrNot(job_num, emp_name))
                        {
                            result.role_speculate = "新员工";
                            result.limited_full_attendance = true;
                            return result;
                        }
                        if (LongHolidayReturnBackFromThisMonth(job_num, emp_name, date_time, outer_list[0].Count))
                        {
                            result.role_speculate = "归来";
                            result.limited_full_attendance = true;
                            return result;
                        }
                    }
                    else
                    {
                        if (LongHolidayJudge(outer_list[0].Count))
                        {
                            result.role_speculate = "长假";
                            result.limited_full_attendance = true;
                            return result;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 控制判断长假的天数
        /// </summary>
        /// <param name="count">天数</param>
        /// <returns></returns>
        public static bool LongHolidayJudge(int count)
        {
            if (count >= Configuration.long_holiday)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取未打卡日期去除掉周末干扰后的连续日期的切片
        /// </summary>
        /// <param name="less_than_workday_list">未打卡日期列表</param>
        /// <param name="weekend_day_list">周末日期列表</param>
        /// <returns></returns>
        public static List<List<int>> GetWorkdaysContinuousSections(List<int> less_than_workday_list, List<int> weekend_day_list)
        {
            List<int> weekend_and_less_than_workday_list = less_than_workday_list.Union(weekend_day_list).ToList();
            weekend_and_less_than_workday_list.Sort();

            //获取包含weekend的切片
            List<List<int>> outer_list_weekend = SliceDaysToContinuousSections(weekend_and_less_than_workday_list);
            //获取不包含weekend的切片
            List<List<int>> outer_list = GetDaysSectionsAfterFiltration(outer_list_weekend, weekend_day_list);

            return outer_list;
        }

        /// <summary>
        /// 把日期列表依照日期连续性进行切片
        /// </summary>
        /// <param name="list">日期列表</param>
        /// <returns></returns>
        public static List<List<int>> SliceDaysToContinuousSections(List<int> list)
        {
            List<List<int>> outer = new List<List<int>>();
            List<int> inner = new List<int>();

            //第一个要单独添加
            inner.Add(list[0]);

            int i = 1;
            while (true)
            {
                if (list[i] - list[i - 1] == 1)
                {
                    inner.Add(list[i]);
                }
                else
                {
                    outer.Add(inner);
                    inner = new List<int>();
                    inner.Add(list[i]);
                }
                if (i <= list.Count() - 2)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            //最后一个需要单独添加
            outer.Add(inner);

            return outer;
        }

        /// <summary>
        /// 对日期切片进行过滤,例如过滤掉weekend
        /// </summary>
        /// <param name="outer_original">原来的日期切片列表</param>
        /// <param name="filter_list">过滤列表</param>
        /// <returns></returns>
        public static List<List<int>> GetDaysSectionsAfterFiltration(List<List<int>> outer_original,List<int> filter_list)
        {
            List<List<int>> outer = new List<List<int>>();

            for (int j = 0; j < outer_original.Count(); j++)
            {
                List<int> list = outer_original[j];
                List<int> list2 = list.Except(filter_list).ToList<int>();
                if (list2.Count() != 0)
                {
                    outer.Add(list2);
                }
            }
            return outer;
        }
    }
}
