using Attendance.Model.DataBase;
using Attendance.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Attendance.Controller.Analysis
{
    class analysisOfMonth
    {
        /// <summary>
        /// 分析某个员工对象一个月的打卡数据，用以获得一些整体性的无法从单日里面获取的信息
        /// </summary>
        /// <param name="emp_month_infos">目标员工对象一个月的打卡数据</param>
        /// <param name="workdayList">某月的工作日列表</param>
        /// <param name="weekend_day_List">某月的周末日期列表(排除周末加班)</param>
        /// <returns>某员工月度打卡数据分析结果</returns>
        public static AnalysisOfMonth GetAnalysisOfMonth(List<AttendanceInfo> one_emp_month_infos, List<int> work_day_list, List<int> weekend_day_List)
        {
            AnalysisOfMonth analysis_of_month = new AnalysisOfMonth();
            AnalysisOfMonthData analysis_of_month_data = GetAnalysisOfMonthData(one_emp_month_infos);

            //此员工所有打卡日期列表
            List<int> punch_card_day_list = analysis_of_month_data.day_list;

            //打卡日期比工作日期少的日期所组成的list
            List<int> less_than_workday_list = work_day_list.Except(punch_card_day_list).ToList();
            //打卡日期比工作日期多的日期所组成的list
            List<int> more_than_workday_list = punch_card_day_list.Except(weekend_day_List).ToList();

            //缺勤打卡次数
            analysis_of_month.absence_punch_card_day_count = less_than_workday_list.Count;
            //超勤打卡天数
            analysis_of_month.exceed_punch_card_day_count = more_than_workday_list.Count;

            //差满勤日期列表
            //analysis_of_month.less_than_workday_list = less_than_workday_list;

            analysis_of_month.full_attendance = false;
            //所有工作日都打卡的是满勤
            if (less_than_workday_list.Count == 0)
            {
                analysis_of_month.full_attendance = true;
            }
            else
            {

                //将周末和未打卡的工作日日期列表进行拼接
                List<int> weekend_and_less_than_workday_list = less_than_workday_list.Union(weekend_day_List).ToList<int>();
                weekend_and_less_than_workday_list.Sort();

                //进行切块,获取包含weekend的切片
                List<List<int>> outer_list_weekend = new List<List<int>>();
                List<int> inner_list_weekend = new List<int>();

                //第一个要单独添加
                inner_list_weekend.Add(weekend_and_less_than_workday_list[0]);

                int i = 1;
                while (true)
                {
                    if (weekend_and_less_than_workday_list[i] - weekend_and_less_than_workday_list[i - 1] == 1)
                    {
                        inner_list_weekend.Add(weekend_and_less_than_workday_list[i]);
                    }
                    else
                    {
                        outer_list_weekend.Add(inner_list_weekend);
                        inner_list_weekend = new List<int>();
                        inner_list_weekend.Add(weekend_and_less_than_workday_list[i]);
                    }
                    if (i < weekend_and_less_than_workday_list.Count() - 1)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                //最后一个需要单独添加
                outer_list_weekend.Add(inner_list_weekend);

                //获取不包含weekend的切片
                List<List<int>> outer_list = new List<List<int>>();
                for (int j = 0; j < outer_list_weekend.Count(); j++)
                {
                    List<int> list = outer_list_weekend[j];
                    List<int> list2 = list.Except(weekend_day_List).ToList<int>();
                    if (list2.Count() != 0)
                    {
                        outer_list.Add(list2);
                    }
                }

                //对切片进行分析
                //假如有五个以上的碎片化没打卡时间,这个有可能是特殊人员
                if (outer_list.Count() >= 5)
                {
                    analysis_of_month.attendance_speculate = "特殊员工";
                }
                else if (outer_list[0].Contains(1))
                {
                    //1.仅仅是一号请假的意外情况,这就会导致二号入职的人员未被归纳入新员工,这是个特殊情况,将来要通过查询之前的数据来避免
                    if (outer_list[1].Count == 1)
                    {
                        analysis_of_month.attendance_speculate = "普通";
                    }
                    else
                    {
                        //推测新员工是为了为满勤考量
                        analysis_of_month.attendance_speculate = "新员工";
                        //如果只有一个以一号为起始的未打卡片段,那么推测为新员工
                        if (outer_list.Count == 1)
                        {
                            analysis_of_month.limited_full_attendance = true;
                        }
                    }
                }
                else
                {
                    analysis_of_month.attendance_speculate = "普通";
                }

                foreach (var snippet in outer_list)
                {
                    if (snippet.Count > 3)
                    {
                        analysis_of_month.exist_long_holiday = true;
                    }
                }

            }

            //当日首次打卡hour列表
            List<int> start_hour_list = analysis_of_month_data.first_and_last_punch_card_hour_list.Select(t => t.start_hour).ToList<int>();
            List<int> end_hour_list = analysis_of_month_data.first_and_last_punch_card_hour_list.Select(t => t.end_hour).ToList<int>();

            var start_hour_unique = start_hour_list.Distinct().ToList<int>();
            var end_hour_unique = end_hour_list.Distinct().ToList<int>();

            //if (start_hour_unique.Count == 1)
            //{
            //    analysis_of_month.shift = start_hour_list[0] + "";
            //}
            //else if ()
            //{

            //}



            return analysis_of_month;
        }


        /// <summary>
        /// 获取某个员工对象一个月的打卡数据然后进行初步分析,所得的数据目的用于进一步分析
        /// 不受其他数据干扰的数据大多都在这里获取,以降低代码的复杂性,例如在这里获取
        /// 本员工本月的所有打卡日期列表,提供给进一步分析的方法用来和工作日,节假日
        /// 进行比对,或者和之前的数据互相印证看是否是新员工等
        /// </summary>
        /// <param name="emp_month_infos">目标员工对象一个月的打卡数据</param>
        /// <returns>某员工月度打卡数据初步分析</returns>
        public static AnalysisOfMonthData GetAnalysisOfMonthData(List<AttendanceInfo> one_emp_month_infos)
        {
            AnalysisOfMonthData analysis_of_month_data = new AnalysisOfMonthData();
            AttendanceInfo first_day_info = one_emp_month_infos.First();

            analysis_of_month_data.year = first_day_info.inout_time.Year;
            analysis_of_month_data.month = first_day_info.inout_time.Month;
            analysis_of_month_data.card_id = first_day_info.card_id;
            analysis_of_month_data.job_num = first_day_info.job_num;
            analysis_of_month_data.emp_name = first_day_info.emp_name;
            analysis_of_month_data.punch_card_count_month = one_emp_month_infos.Count();

            //所有打卡了的日期
            HashSet<int> daySet = new HashSet<int>();
            foreach (var atten in one_emp_month_infos)
            {
                daySet.Add(atten.inout_time.Day);
            }
            //打卡日期列表(由于从数据库中读值的时候已经排过序了,所以这里的列表里面的值也是有序的)
            analysis_of_month_data.day_list = daySet.ToList();


            PunchCardHourDistinguish punch_card_hour_distinguish = GetFirstAndLastPunchCardHour(one_emp_month_infos);

            //此员工本月所有早晚打卡小时值


            return analysis_of_month_data;
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
            List<FirstAndLastPunchCardHour> punch_card_hour_list_normal = new List<FirstAndLastPunchCardHour>();
            //本月所有早晚打卡小时值(不正常的,只有一次打卡记录)
            List<FirstAndLastPunchCardHour> punch_card_hour_list_only_one = new List<FirstAndLastPunchCardHour>();

            //对此员工本月的打卡数据按照每日进行切片
            for (int i = 1; i <= 31; i++)
            {
                FirstAndLastPunchCardHour punch_card_hour = new FirstAndLastPunchCardHour();

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

                    punch_card_hour_list_normal.Add(punch_card_hour);

                }else if (one_emp_day_infos.Count==1)   //只有一次打卡记录,这是不正常的
                {
                    punch_card_hour.day = start_time.Day;

                    //当天第一次打卡时间的小时值(分钟值:0-30视为本小时,31-59.59视为下一个小时)
                    timespan = one_emp_day_infos.First().inout_time.TimeOfDay;
                    minutes = timespan.Minutes % 60;
                    hour = minutes > 30 ? timespan.Hours + 1 : timespan.Hours;
                    punch_card_hour.start_hour = hour;
                    punch_card_hour.end_hour = 0;
                    punch_card_hour_list_only_one.Add(punch_card_hour);
                }

                //跳转到下一天
                start_time = start_time.AddDays(1);
                end_time = end_time.AddDays(1);
            }

            PunchCardHourDistinguish result = new PunchCardHourDistinguish();
            result.normal = punch_card_hour_list_normal;
            result.only_one = punch_card_hour_list_only_one;
            return result;
        }
    }
}
