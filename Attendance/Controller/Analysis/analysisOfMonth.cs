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
        /// <param name="weekend_day_list">某月的周末日期列表(排除周末加班)</param>
        /// <returns>某员工月度打卡数据分析结果</returns>
        public static AnalysisOfMonth GetAnalysisOfMonth(List<AttendanceInfo> one_emp_month_infos, List<int> work_day_list, List<int> weekend_day_list, List<AnalysisOfDay> analysis_of_day_list)
        {
            //员工月度早晚打卡小时数据
            PunchCardHourDistinguish punch_card_hour_distinguish = analysisUtil.GetFirstAndLastPunchCardHour(one_emp_month_infos);
            List<PunchCardHour> first_and_last_punch_card_hours_all = punch_card_hour_distinguish.all;
            List<PunchCardHour> first_and_last_punch_card_hours_normal = punch_card_hour_distinguish.normal;
            List<PunchCardHour> first_and_last_punch_card_hours_only_one = punch_card_hour_distinguish.only_one;

            AnalysisOfMonth analysis_of_month = new AnalysisOfMonth();
            AttendanceInfo first_day_info = one_emp_month_infos.First();

            analysis_of_month.date = new DateTime(first_day_info.inout_time.Year,first_day_info.inout_time.Month,1);   //日期(年月)
            analysis_of_month.card_id = first_day_info.card_id;             //卡号
            analysis_of_month.job_num = first_day_info.job_num;             //工号
            analysis_of_month.emp_name = first_day_info.emp_name;           //姓名
            analysis_of_month.punch_card_day_count = first_and_last_punch_card_hours_all.Count();
            foreach (var analysis_of_day in analysis_of_day_list)
            {
                //不包括一天只打一次卡的
                if (analysis_of_day.punch_card_count_total>1)
                {
                    //总打卡数(不包括一天只打一次卡的),之所以这么做,是因为后面日平均打卡数要用到这个数据
                    analysis_of_month.punch_card_count_valid_month += analysis_of_day.punch_card_count_valid;       
                }
            }            
            foreach (var day in first_and_last_punch_card_hours_normal)
            {
                analysis_of_month.attendance_time_of_month += day.time_of_duration;     //所有正常打卡日在公司的总时间
            }
            if (first_and_last_punch_card_hours_normal.Count() > 0)             //以防某个员工一个月内没有一天有两次以上打卡的极端情况
            {
                analysis_of_month.punch_card_count_valid_day = analysis_of_month.punch_card_count_valid_month / (double)first_and_last_punch_card_hours_normal.Count();    //打卡日里的平均打卡次数

                AttendanceNormalAndShort attendance_normal_and_short = analysisUtil.GetAttendanceNormalAndShort(first_and_last_punch_card_hours_normal);
                analysis_of_month.attendance_time_of_day_normal = attendance_normal_and_short.attendance_time_of_day_normal;
                analysis_of_month.attendance_time_of_day_normal_count = attendance_normal_and_short.attendance_time_of_day_normal_count;
                analysis_of_month.attendance_time_of_day_short = attendance_normal_and_short.attendance_time_of_day_short;
                analysis_of_month.attendance_time_of_day_short_count = attendance_normal_and_short.attendance_time_of_day_short_count;
            }

            List<int> punch_card_day_list_all = first_and_last_punch_card_hours_all.Select(t=>t.day).ToList();
            List<int> punch_card_day_list_only_one = first_and_last_punch_card_hours_only_one.Select(t => t.day).ToList();
            List<int> punch_card_day_hour_list_only_one = first_and_last_punch_card_hours_only_one.Select(t => t.start_hour).ToList();
            List<int> less_than_workday_list = work_day_list.Except(punch_card_day_list_all).ToList();  //打卡日期比工作日期少的日期所组成的list
            List<int> more_than_workday_list = punch_card_day_list_all.Except(work_day_list).ToList();   //打卡日期比工作日期多的日期所组成的list
            analysis_of_month.absence_punch_card_day_count = less_than_workday_list.Count; //缺勤打卡次数
            analysis_of_month.exceed_punch_card_day_count = more_than_workday_list.Count;   //超勤打卡次数
            if (analysis_of_month.exceed_punch_card_day_count > 0)
            {
                analysis_of_month.exceed_punch_card_day_list = string.Join(",", more_than_workday_list.ToList());
            }
            analysis_of_month.has_only_one_punch_card_day = punch_card_day_list_only_one.Count > 0 ? true : false;
            analysis_of_month.only_one_punch_card_day_list = string.Join(",", punch_card_day_list_only_one);
            analysis_of_month.only_one_punch_card_day_hour_list = string.Join(",", punch_card_day_hour_list_only_one);
            //这里都用正常打卡日(至少两次打卡,排除一次打卡),因为单次打卡日要首先要判断此次打卡是初始打卡还是最后打卡,
            //再者由于其只有一次打卡导致接下来的打卡小时变更次数统计那里由于缺少相应的记录而无所适从,所以干脆独立出来.
            analysis_of_month.start_hour_unique_list = string.Join(",", first_and_last_punch_card_hours_normal.OrderBy(t=>t.start_hour).Select(t=>t.start_hour).Distinct().ToList());
            analysis_of_month.end_hour_unique_list = string.Join(",", first_and_last_punch_card_hours_normal.OrderBy(t => t.end_hour).Select(t => t.end_hour).Distinct().ToList());

            StartAndEenHourChangeTimes start_and_een_hour_change_times = analysisUtil.GetStartAndEenHourChangeTimes(first_and_last_punch_card_hours_normal);
            analysis_of_month.start_hour_change_times = start_and_een_hour_change_times.start_hour_change_times;
            analysis_of_month.end_hour_change_times = start_and_een_hour_change_times.end_hour_change_times;

            //获取是否满勤以及身份推测等相关信息
            FullAttendanceAndRoleSpeculate full_attendance_and_role_speculate = analysisUtil.GetFullAttendanceAndRoleSpeculate(less_than_workday_list,weekend_day_list,analysis_of_month.job_num,analysis_of_month.emp_name,analysis_of_month.date);
            analysis_of_month.full_attendance = full_attendance_and_role_speculate.full_attendance;
            analysis_of_month.role_speculate = full_attendance_and_role_speculate.role_speculate;
            analysis_of_month.limited_full_attendance = full_attendance_and_role_speculate.limited_full_attendance;
            analysis_of_month.absence_sections_count = full_attendance_and_role_speculate.absence_sections.Count();
            if(analysis_of_month.absence_punch_card_day_count > 0)
            {
                foreach (var one_section in full_attendance_and_role_speculate.absence_sections)
                {
                    analysis_of_month.absence_punch_card_day_list += string.Join(",", one_section) + ";";
                }
            }

            analysis_of_month.exist_long_holiday = full_attendance_and_role_speculate.long_holiday;

            return analysis_of_month;
        }
    }
}
