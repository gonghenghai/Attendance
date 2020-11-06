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
            AttendanceInfo first_day_info = one_emp_month_infos.First();

            //员工一天内的所有打卡数据
            List<AttendanceInfo> one_emp_day_infos = new List<AttendanceInfo>();
            //用于切片每天的时间，以早上四点为临界点，每天的时间段为凌晨四点到次日凌晨四点
            DateTime start_time = new DateTime(first_day_info.inout_time.Year, first_day_info.inout_time.Month, 1, 4, 0, 0);
            DateTime end_time = new DateTime(first_day_info.inout_time.Year, first_day_info.inout_time.Month, 2, 4, 0, 0);
            TimeSpan timespan = new TimeSpan();
            int minutes;
            int hour;
            //本月所有早晚打卡小时值(正常的,有两次以上打卡)
            List<PunchCardHour> punch_card_hour_list_normal = new List<PunchCardHour>();
            //本月所有早晚打卡小时值(不正常的,只有一次打卡记录)
            List<PunchCardHour> punch_card_hour_list_only_one = new List<PunchCardHour>();
            //本月所有早晚打卡小时值
            List<PunchCardHour> punch_card_hour_list_all = new List<PunchCardHour>();

            //对此员工本月的打卡数据按照每日进行切片
            for (int i = 1; i <= 31; i++)
            {
                PunchCardHour punch_card_hour = new PunchCardHour();

                //当天内的所有打卡记录
                one_emp_day_infos = one_emp_month_infos.Where(x => x.inout_time > start_time && x.inout_time < end_time).ToList();

                //正常的打卡次数,至少包括两次
                if (one_emp_day_infos.Count >= 2)
                {
                    punch_card_hour.day = start_time.Day;

                    //当天第一次打卡时间的小时值(分钟值:0-30视为本小时,31-59.59视为下一个小时)
                    timespan = one_emp_day_infos.First().inout_time.TimeOfDay;
                    minutes = timespan.Minutes % 60;
                    hour = minutes > 30 ? timespan.Hours + 1 : timespan.Hours;
                    punch_card_hour.start_hour = hour;

                    //当天最后一次打卡时间的小时值(分钟值:0-30视为本小时,31-59.59视为下一个小时)
                    timespan = one_emp_day_infos.Last().inout_time.TimeOfDay;
                    minutes = timespan.Minutes % 60;
                    hour = minutes > 30 ? timespan.Hours + 1 : timespan.Hours;
                    punch_card_hour.end_hour = hour;

                    //最后一次打卡和第一次打卡之间的时间间隔
                    timespan = one_emp_day_infos.Last().inout_time.TimeOfDay - one_emp_day_infos.First().inout_time.TimeOfDay;
                    punch_card_hour.time_of_duration = timespan;

                    punch_card_hour_list_normal.Add(punch_card_hour);
                    punch_card_hour_list_all.Add(punch_card_hour);

                }
                else if (one_emp_day_infos.Count == 1)   //只有一次打卡记录,这是不正常的
                {
                    punch_card_hour.day = start_time.Day;

                    //当天第一次打卡时间的小时值(分钟值:0-30视为本小时,31-59.59视为下一个小时)
                    timespan = one_emp_day_infos.First().inout_time.TimeOfDay;
                    minutes = timespan.Minutes % 60;
                    hour = minutes > 30 ? timespan.Hours + 1 : timespan.Hours;
                    punch_card_hour.start_hour = hour;
                    punch_card_hour.end_hour = 0;
                    punch_card_hour.time_of_duration = new TimeSpan(0);

                    punch_card_hour_list_only_one.Add(punch_card_hour);
                    punch_card_hour_list_all.Add(punch_card_hour);
                }

                //跳转到下一天
                start_time = start_time.AddDays(1);
                end_time = end_time.AddDays(1);
            }

            PunchCardHourDistinguish result = new PunchCardHourDistinguish();
            result.normal = punch_card_hour_list_normal;
            result.only_one = punch_card_hour_list_only_one;
            result.all = punch_card_hour_list_all;
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
                result.Add(one_emp_day_infos);

                //跳转到下一天
                start_time = start_time.AddDays(1);
                end_time = end_time.AddDays(1);
            }

            return result;
        }

        public static AnalysisOfDay GetAnalysisResultOfOneDay(List<AttendanceInfo> day_infos)
        {
            AnalysisOfDay analysis_of_day = new AnalysisOfDay();

            AttendanceInfo first_info = day_infos.First();
            AttendanceInfo last_info = day_infos.Last();

            analysis_of_day.date = first_info.inout_time.Date;
            analysis_of_day.week = Convert.ToInt32(first_info.inout_time.DayOfWeek);
            analysis_of_day.card_id = first_info.card_id;
            analysis_of_day.job_num = first_info.job_num;
            analysis_of_day.emp_name = first_info.emp_name;
            analysis_of_day.first_tm = first_info.inout_time;
            analysis_of_day.last_tm = last_info.inout_time;

            TimeSpan timespan = first_info.inout_time.TimeOfDay;
            analysis_of_day.first_hr= timespan.Minutes % 60 > 30 ? timespan.Hours + 1 : timespan.Hours;
            timespan = last_info.inout_time.TimeOfDay;
            analysis_of_day.last_hr = timespan.Minutes % 60 > 30 ? timespan.Hours + 1 : timespan.Hours;
            analysis_of_day.shift = GetShift(analysis_of_day.first_hr);

            FirstAndLastPunchCardAnalysisResult first_and_last_punch_card_analysis_result = GetFirstAndLastPunchCardAnalysis(analysis_of_day.first_tm, analysis_of_day.last_tm, analysis_of_day.first_hr, analysis_of_day.last_hr, analysis_of_day.shift);


            return analysis_of_day;
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
            int standard_work_hour = 9;
            //晚班很少有人能坚持到九个小时,所以可以灵活配置,比如降低到8
            if (shift == "中班") standard_work_hour = 8;

            //初次打卡缓冲分钟数.举例:这样将使得9:00到9:05分以内的时间被统一视为9:00
            int arrive_buffer_minutes = 5;
            //最后打卡缓冲分钟数.举例:这样将使得5:55到6:00的时间被统一视为6:00
            int leave_buffer_minutes = 5;
            //初次打卡边界分钟数,这个分钟数将用来判定晚到,早离.举例:9:05到9:25将被视为晚到,但不算是迟到,超过9:25的就算是迟到了
            int arrive_limit_minutes = 25;
            //最后打卡边界分钟数,这个分钟数将用来判定迟到,早退.举例:5:45到5:55将被视为早离,但不算是早退,但在5:45之前的就算是早退了
            int leave_limit_minutes = 15;

            FirstAndLastPunchCardAnalysisResult result = new FirstAndLastPunchCardAnalysisResult();

            TimeSpan standard_work_time = new TimeSpan(standard_work_hour, 0, 0);
            int punch_card_hour = last_hr - first_hr;
            TimeSpan punch_card_time = last_tm - first_tm;
            int punch_card_hour_compare = punch_card_hour - standard_work_hour;
            int punch_card_minute_compare = (punch_card_time - standard_work_time).Minutes;

            result.punch_card_hour = punch_card_hour;
            result.punch_card_time = punch_card_time;
            result.punch_card_hour_compare = punch_card_hour_compare;
            result.punch_card_minute_compare = punch_card_minute_compare;

            TimeSpan first_tm_gap = new DateTime(first_tm.Year, first_tm.Month, first_hr) - first_tm;
            TimeSpan last_tm_gap = last_tm - new DateTime(first_tm.Year, first_tm.Month, last_hr);

            TimeSpan standard_first_tm_gap = new DateTime(first_tm.Year, first_tm.Month, 9) - first_tm;
            TimeSpan standard_last_tm_gap = last_tm- new DateTime(first_tm.Year, first_tm.Month, 9 + standard_work_hour);
            if(shift=="中班")
                standard_first_tm_gap = new DateTime(first_tm.Year, first_tm.Month, 15) - first_tm;
                standard_last_tm_gap = last_tm - new DateTime(first_tm.Year, first_tm.Month, 15 + standard_work_hour);

            TimeSpan arrive_buffer = new TimeSpan(0, -arrive_buffer_minutes, 0);
            TimeSpan leave_buffer = new TimeSpan(0, -leave_buffer_minutes, 0);
            TimeSpan arrive_limit = new TimeSpan(0, -arrive_limit_minutes, 0);
            TimeSpan leave_limit = new TimeSpan(0, -leave_limit_minutes, 0);

            bool first_tm_buffer_gap_meet = first_tm_gap >= arrive_buffer;
            bool first_tm_limit_gap_meet = first_tm_gap >= arrive_limit && first_tm_gap < arrive_buffer;
            bool first_tm_limit_gap_not_meet = first_tm_gap < arrive_limit;

            bool last_tm_buffer_gap_meet = last_tm_gap >= leave_buffer;
            bool last_tm_limit_gap_meet = last_tm_gap >= leave_limit && last_tm_gap < leave_buffer;
            bool last_tm_limit_gap_not_meet = last_tm_gap < leave_limit;

            bool standard_last_tm_buffer_gap_meet = standard_last_tm_gap >= leave_buffer;
            bool standard_last_tm_limit_gap_meet = standard_last_tm_gap >= leave_limit && standard_last_tm_gap < leave_buffer;
            bool standard_last_tm_limit_gap_not_meet = standard_last_tm_gap < leave_limit;

            bool enough_hour_equal = punch_card_hour == standard_work_hour;
            bool enough_hour_more = punch_card_hour > standard_work_hour;
            bool enough_hour_equal_more = punch_card_hour >= standard_work_hour;

            bool first_hour_in_range = FirstHourInRange(shift, first_hr);

            /*
             
            获取中间块

            早班:
            计算初始打卡时间与早班最迟时间九点的差值
             
             */

            /*
            初始判断
            默认以30分为划分线获取的小时值代表准确的小时信息:
            初始打卡时间为早上7:26,默认判定为迟到26分钟,忽略早到34分钟,早到1小时34分钟
            初始打卡时间为早上8:35,默认判定为早上
            初始打卡为下午2:28来其实是早来32分钟的情况,而是直接归类为迟到28分钟,
            初始打卡为早上8:35其实是晚到35分钟,而是归类为早到25分钟.

            完美 初始时间在正确区间, 小时够, first_tm_buffer_gap_meet,last_tm_buffer_gap_meet

            晚到 初始时间在正确区间, first_tm_limit_gap_meet

            迟到 初始时间不在正确区间
            迟到 first_tm_limit_gap_not_meet

            早离 初始时间在正确区间,小时数刚好, last_tm_limit_gap_meet
            早离 初始时间不在正确区间,standard_last_tm_limit_gap_meet

            早退 初始时间在正确区间,小时数不足,last_tm_limit_gap_not_meet
            早退 初始时间不在正确区间,standard_last_tm_limit_gap_not_meet

             */
            /*
            再过滤:
            用标准时间再算一遍,例如一个人下午三点上班,但是他下午2:28来,现在的逻辑就会导致他晚到,但是有谁会提前那么多时间来呢
            但是还是需要在之后再加一层过滤

             */

            /*
            首先看时长,早晚打卡之间的时长,
            然后看初始打卡小时是否落在正确的区间
             
             */



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

        public static StartAndEenHourChangeTimes GetStartAndEenHourChangeTimes(List<PunchCardHour> all, List<PunchCardHour> normal)
        {
            StartAndEenHourChangeTimes result = new StartAndEenHourChangeTimes();

            int all_changes = 0;
            int normal_changes = 0;
            for(int i = 0; i < all.Count; i++)
            {
                if (all[i + 1].start_hour - all[i].start_hour != 0)
                    all_changes++;
            }
            for(int j = 0; j < normal.Count; j++)
            {
                if (normal[j + 1].end_hour - normal[j].end_hour != 0)
                    normal_changes++;
            }

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
            result.absence_sections_count = 0;
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

                result.absence_sections_count = outer_list.Count;

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
                        if (LongHolidayReturnBackFromThisMonth(job_num, emp_name, date_time, outer_list[1].Count))
                        {
                            result.role_speculate = "归来";
                            result.limited_full_attendance = true;
                            return result;
                        }
                    }
                    else
                    {
                        if (LongHolidayJudge(outer_list[1].Count))
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
            if (count >= 5)
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
